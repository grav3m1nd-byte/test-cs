using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication {

    [Serializable]
    public class Profile {

        public string Name { get; set; }
        public string Color { get; set; }
        public string Language { get; set; }
    }
}
