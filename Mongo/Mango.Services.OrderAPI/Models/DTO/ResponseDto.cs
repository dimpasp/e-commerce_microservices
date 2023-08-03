﻿namespace Mango.Services.OrderAPI.Models.DTO
{
    public class ResponseDto
    {
        /// <summary>
        /// 
        /// </summary>
        public object? Result { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// set default value true
        /// </summary>
        public bool IsSuccess { get; set; } = true; 
    }
}
