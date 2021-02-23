using System;
using System.ComponentModel;

namespace Model
{
    public class TeamDto
    {
        [DisplayName("Team ID")]
        public Guid TeamID { get; set; }
        [DisplayName("Team Name")]
        public string Name { get; set; }
        [DisplayName("Wins")]
        public int Wins { get; set; }
        [DisplayName("Losses")]
        public int Losses { get; set; }
        [DisplayName("Carpool ID")]
        public Guid CarpoolID { get; set; }
        public Guid LeagueID { get; set; }
        public Guid StatLineID { get; set; }
    }
}
