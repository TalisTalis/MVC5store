﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_magic_store.Models.Data
{
    [Table("tblProducts")] // К какой таблице относится
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public string ImageName { get; set; }

        // Внешний ключ. Связь с другой таблицей
        [ForeignKey("CategoryId")]
        public virtual CategoryDTO Category { get; set; }
    }
}