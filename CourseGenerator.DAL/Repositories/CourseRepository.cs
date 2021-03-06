﻿using CourseGenerator.DAL.Context;
using CourseGenerator.DAL.Interfaces;
using CourseGenerator.DAL.Pagination;
using CourseGenerator.Models.Entities.CourseAccess;
using CourseGenerator.Models.Entities.Info;
using CourseGenerator.Models.Entities.InfoByThemes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseGenerator.DAL.Repositories
{
    public class CourseRepository : GenericEFRepository<Course>, ICourseRepository
    {
        public CourseRepository(ApplicationContext context) : base(context) {}

        /// <summary>
        /// Вибирає курси з вказаною мовою для вказаного користувача з пагінацією
        /// </summary>
        /// <param name="userId"><c>Id</c> користувача, для якого ми отримуєм курси</param>
        /// <param name="langCode">Курси вибраною мовою мають пріорітет</param>
        /// <param name="pageSize">Кількість об'єктів на сторінці</param>
        /// <param name="pageIndex">Номер сторінки</param>
        /// <returns><c>PagedList з об'єктами</c> <c>CourseLang</c></returns>
        /// <remarks>
        /// <para>
        /// Для таблиці <c>CourseLangs</c>, в якій звязується курс з мовою,
        /// підгружаємо дані про мови і вибираємо об'єкти, 
        /// які є в результатах підзапиту:
        /// </para>
        /// <para>
        /// з таблиці <c>UserCourses</c>, де зв'язується курс та користувач
        /// вибираємо ті курси, а яких <c>UserId</c> рівне id користувача, 
        /// для якого вибираємо, далі вибираються id курсів, і якщо 
        /// <c>CourseId</c> з таблиці <c>CourseLangs</c> є в результатах даного підзапиту
        /// то відбираємо такий об'єкт <c>CourseLang</c>.
        /// </para>
        /// <para>
        /// Далі групуємо результати
        /// за <c>CourseId</c> (бо зараз вибрані версії кожного курсу всіма доступними
        /// мовами). І з кожної групи вибираємо <c>CourseLang</c> з потрібною нам мовою
        /// або першою доступною (якщо немає потрібною).
        /// </para>
        /// </remarks>
        public async Task<PagedList<CourseLang>> GetForUserWithLangPagedAsync(
            string userId, string langCode, int pageSize, int pageIndex)
        {
            IQueryable<CourseLang> coursesForUser = _context.CourseLangs
                .Include(cl => cl.Lang)
                .Where(cl =>
                    _context.UserCourses
                    .Where(uc => uc.UserId == userId)
                    .Select(uc => uc.CourseId)
                    .Contains(cl.CourseId));

            IQueryable<CourseLang> coursesWithSpecifiedLang = coursesForUser
                .Where(cl => cl.Lang.Code == langCode);

            IQueryable<CourseLang> coursesWithFirstLang = coursesForUser
                .Where(cl => !coursesWithSpecifiedLang
                .Select(cl => cl.CourseId)
                .Contains(cl.CourseId));

            IQueryable<CourseLang> coursesForUserWithLang = coursesForUser.Union(coursesWithSpecifiedLang);

            return await coursesForUserWithLang.OrderBy(p => p.Name).ToPagedListAsync(pageSize, pageIndex);
        }

        public async Task<IEnumerable<CourseLang>> GetForUserWithLangAsync(
            string userId, string langCode)
        {
            IQueryable<CourseLang> coursesForUser = _context.CourseLangs
                .Include(cl => cl.Lang)
                .Where(cl =>
                    _context.UserCourses
                    .Where(uc => uc.UserId == userId)
                    .Select(uc => uc.CourseId)
                    .Contains(cl.CourseId));

            IQueryable<CourseLang> coursesWithSpecifiedLang = coursesForUser
                .Where(cl => cl.Lang.Code == langCode);

            IQueryable<CourseLang> coursesWithFirstLang = coursesForUser
                .Where(cl => !coursesWithSpecifiedLang
                .Select(cl => cl.CourseId)
                .Contains(cl.CourseId));

            IQueryable<CourseLang> coursesForUserWithLang = coursesForUser.Union(coursesWithSpecifiedLang);

            return await coursesForUserWithLang.OrderBy(p => p.Name).ToListAsync();
        }

        public async Task<int?> GetLastThemeIdOrNullAsync(
            string userId, int courseId)
        {
            UserCourse course = await _context.UserCourses
                .FirstOrDefaultAsync(uc => uc.UserId == userId &&
                uc.CourseId == courseId);

            return course?.LastThemeId;
        }

        //public async Task<IEnumerable<LevelLang>> GetLevelByCourseIdAsync(int courseId)
        //{
        //    IQueryable<Level> levelCourses = _context.Themes
        //        .Where(lc => lc.CourseId == courseId)
        //        .Select(lc => lc.Level);

            
        //}
    }
}
