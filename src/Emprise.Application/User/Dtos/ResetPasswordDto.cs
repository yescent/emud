﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Emprise.Application.User.Dtos
{
    public class ResetPasswordDto
    {
        [EmailAddress(ErrorMessage = "邮箱格式错误")]
        [Required(ErrorMessage = "请输入邮箱")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Code { get; set; }
    }
}
