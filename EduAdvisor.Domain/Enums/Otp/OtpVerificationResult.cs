using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduAdvisor.Domain.Enums.Otp
{
    public enum OtpVerificationResult
    {
        Success,
        Invalid,
        Expired,
        LockedOut
    }


}
