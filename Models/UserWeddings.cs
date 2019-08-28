using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
    
namespace Wedding_Planner.Models
{
    public class UserWedding
    {
        [Key]
        public int UserWeddingId {get;set;}

        public int UserId {get;set;}

        public int WeddingId {get;set;}

        public User User {get;set;}

        public Wedding Wedding {get;set;}

        public UserWedding(int UserId, int WeddingId)
        {
            this.UserId = UserId;
            this.WeddingId = WeddingId;
        }
    }
}