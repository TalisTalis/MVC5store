using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace MVC_magic_store.Models.Data
{
    // к какой таблице относится этот класс
    [Table("tblPages")]
    public class PagesDTO
    {
        // поля соответствующие столбцам таблицы. в той же последовательности
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Body { get; set; }
        public int Sorting { get; set; }
        public bool HasSideBar { get; set; }

    }
}