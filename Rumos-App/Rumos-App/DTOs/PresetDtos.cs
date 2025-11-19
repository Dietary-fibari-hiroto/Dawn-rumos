using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rumos_App.DTOs
{
    public class PresetCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public IBrowserFile? File { get; set; } = default!;
    }
}
