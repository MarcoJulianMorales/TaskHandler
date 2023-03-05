﻿using System.ComponentModel.DataAnnotations;

namespace TasksHandler.Models
{
    public class EditTaskDTO
    {
        [Required]
        [StringLength(250)]
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
