﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CourseGenerator.Models.Entities.Info
{
    public class MaterialLang
    {
        public int MaterialId { get; set; }
        public Material Material { get; set; }

        public string Name { get; set; }

        public string LangCode { get; set; }
        public Language Lang { get; set; }
    }
}
