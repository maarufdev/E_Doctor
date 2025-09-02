using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Doctor.Application.DTOs.Common.ExportIllnessDTOs
{
    public sealed record ExportIllnessDTO(
        int IllnessId,
        string IllnessName,
        string Description
    );
}
