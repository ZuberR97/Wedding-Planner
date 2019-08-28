using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
    
namespace Wedding_Planner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId {get;set;}

        [Required]
        [MinLength(2)]
        public string Wedder1 {get;set;}

        [Required]
        [MinLength(2)]
        public string Wedder2 {get;set;}

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date {get;set;}

        [Required]
        [MinLength(2)]
        public string Address {get;set;}

        public int UserId {get;set;}

        public List<UserWedding> TheseUsers {get;set;}
    }
}