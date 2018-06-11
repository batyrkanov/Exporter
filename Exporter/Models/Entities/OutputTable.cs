using System;
using System.ComponentModel.DataAnnotations;

namespace Exporter.Models.Entities
{
    public class OutputTable
    {
        public int Id { get; set; }

        [Display(Name = "Наименование таблицы")]
        [StringLength(200, ErrorMessage = "Длина строки не должна превышать 200 символов")]
        [Required(ErrorMessage = "Заполните поле")]
        public string TableName { get; set; }

        [Display(Name = "Наименование файла выходной таблицы")]
        [StringLength(200, ErrorMessage = "Длина строки не должна превышать 200 символов")]
        [Required(ErrorMessage = "Заполните поле!")]
        public string TableFileName { get; set; }

        public string FileType { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int QueryId { get; set; }

        public virtual SqlQuery SqlQuery { get; set; }
    }
}