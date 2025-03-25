using System;
using System.Collections.Generic;

namespace ph.Core.OS {
    [Serializable]
    public class Email {
        public string sender;
        public string subject;
        public string body;
        public bool isPhishing;
        public string difficulty;
    }

    [Serializable]
    public class EmailList {
        public List<Email> emails;
    }
}
