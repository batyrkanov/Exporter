using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Exporter.Models.Entities
{
    public class SqlQuery
    {
        public int SqlQueryId { get; set; }

        [Display(Name = "Наименование запроса")]
        [StringLength(100, ErrorMessage = "Длина строки не должна превышать 100 символов")]
        [Required(ErrorMessage = "Заполните поле!")]
        public string SqlQueryName { get; set; }

        [Display(Name = "Запрос")]
        [DataType(DataType.MultilineText)]
        //[StringLength(100000, ErrorMessage = "Длина строки не должна превышать 100000 символов")]
        [Required(ErrorMessage = "Заполните поле!")]
        public string SqlQueryContent { get; set; }

        [Display(Name = "Дата создания запроса")]
        public DateTime SqlQueryCreatedDate { get; set; }

        public virtual ICollection<SqlQueryParameter> SqlQueryParameters { get; set; }
    }
}