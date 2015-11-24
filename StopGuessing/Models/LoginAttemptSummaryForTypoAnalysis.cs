﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using StopGuessing.DataStructures;

namespace StopGuessing.Models
{
    public class LoginAttemptSummaryForTypoAnalysis
    {
        public DoubleThatDecaysWithTime Penalty { get; set; }

        public string UsernameOrAccountId { get; set;  }

        /// <summary>
        /// When a login attempt is sent with an incorrect password, that incorrect password is encrypted
        /// with the UserAccount's EcPublicAccountLogKey.  That private key to decrypt is encrypted
        /// wiith the phase1 hash of the user's correct password.  If the correct password is provided in the future,
        /// we can go back and audit the incorrect password to see if it was within a short edit distance
        /// of the correct password--which would indicate it was likely a (benign) typo and not a random guess. 
        /// </summary>
        public string EncryptedIncorrectPassword { get; set; }
    }
}
