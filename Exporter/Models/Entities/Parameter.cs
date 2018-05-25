using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Exporter.Models.Entities
{
    public class Parameter
    {
        public int ParameterId { get; set; }

        [Display(Name = "Наименование параметра для замены")]
        [StringLength(100, ErrorMessage = "Длина строки не должна превышать 100 символов")]
        [Required(ErrorMessage = "Заполните поле!")]
        public string ParameterName { get; set; }

        [Display(Name = "Наименование параметра на русском")]
        [StringLength(100, ErrorMessage = "Длина строки не должна превышать 100 символов")]
        [Required(ErrorMessage = "Заполните поле!")]
        public string ParameterRuName { get; set; }

        [Display(Name = "Тип параметра")]
        [StringLength(100, ErrorMessage = "Длина строки не должна превышать 100 символов")]
        [Required(ErrorMessage = "Заполните поле!")]
        public string ParameterType { get; set; }

        [Display(Name = "Дата создания параметра")]
        public DateTime ParameterCreatedDate { get; set; }

        public virtual ICollection<SqlQueryParameter> SqlQueriesParameters { get; set; }
        

    }
}