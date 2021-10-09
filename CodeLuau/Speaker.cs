using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeLuau
{
    public class Speaker
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? YearsExperience { get; set; }
        public bool HasBlog { get; set; }
        public string BlogURL { get; set; }
        public WebBrowser Browser { get; set; }
        public List<string> Certifications { get; set; }
        public string Employer { get; set; }
        public int RegistrationFee { get; set; }
        public List<Session> Sessions { get; set; }

        public RegisterResponse Register(IRepository repository)
        {
            var error = ValidateData();
            if (error != null) return new RegisterResponse(error);
            return new RegisterResponse(repository.SaveSpeaker(this));
        }

        private bool ApproveSessions()
        {
            foreach (var session in Sessions)
            {
                session.Approved = !SessionIsAboutOldTechnology(session);
            }

            return Sessions.Any(s => s.Approved);
        }

        private bool SessionIsAboutOldTechnology(Session session)
        {
            var oldTechnologies = new List<string>() { "Cobol", "Punch Cards", "Commodore", "VBScript" };
            foreach (var tech in oldTechnologies)
            {
                if (session.Title.Contains(tech) || session.Description.Contains(tech))
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasObviousRedFlags()
        {
            string emailDomain = Email.Split('@').Last();
            var ancientEmailDomains = new List<string>() { "aol.com", "prodigy.com", "compuserve.com" };
            if (ancientEmailDomains.Contains(emailDomain)) return true;
            return Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < 9;
        }

        private bool AppearsExceptional()
        {
            if (YearsExperience > 10) return true;
            if (HasBlog) return true;
            if (Certifications.Count() > 3) return true;
            var preferredEmployers = new List<string>() { "Pluralsight", "Microsoft", "Google" };
            return preferredEmployers.Contains(Employer);
        }

        public RegisterError? ValidateData()
        {
            if (string.IsNullOrWhiteSpace(FirstName)) return RegisterError.FirstNameRequired;
            if (string.IsNullOrWhiteSpace(LastName)) return RegisterError.LastNameRequired;
            if (string.IsNullOrWhiteSpace(Email)) return (RegisterError?)RegisterError.EmailRequired;
            if (!Sessions.Any()) return RegisterError.NoSessionsProvided;
            if (!(AppearsExceptional() || !HasObviousRedFlags())) return RegisterError.SpeakerDoesNotMeetStandards;
            if (!ApproveSessions()) return RegisterError.NoSessionsApproved;
            return null;
        }
    }
}