using MagicVilla_VillaAPI.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Data
{
    public static class VillaStore
    {
        public static List<VillaDTO> villaList = new List<VillaDTO>
        {
            new VillaDTO
            {
                Id = 1,
                Name = "Pool View",
                Occupancy=3,
                Sqft=100
            },
            new VillaDTO
            {
                Id = 2,
                Name = "Beach View",
                Occupancy=4,
                Sqft=200
            }
        };
    }
}
