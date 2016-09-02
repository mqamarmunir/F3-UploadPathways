using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace UploadPathwaysReports
{
    public class clsPathWaysCaseRecord
    {
        public int siteId;
        public string supplierCallRef;
        public int ageGroupId;
        // public int ageInYears;
        public int genderId;
        public int partyId;
        public string postcode;
        public string chUserCode;
        public int chSkillsetId;
        public string chCallStartTime;
        public string chTriageStartTime;
        public string chDispoTime;
        public string chTriageEndTime;
        public string chCallEndTime;
        public string chTriageDispoCode;
        public string chFinalDispoCode;
        //public string chOverrideFlag;
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

        public string clUserCode;
        public int clSkillsetId;
        public string clCallStartTime;
        public string clTriageStartTime;
        public string clDispoTime;
        public string clTriageEndTime;
        public string clCallEndTime;
        public string clTriageDispoCode;
        public string clFinalDispoCode;
        //public string clOverrideFlag;

        //public string symptomGroup;
        //public string symptomDiscriminator;
        public string releaseVersion;
        //public int clinicalAreaId;
        //public string callerType;
        //public string callReason;
        //public int dosTransactionId;
        //public DateTime dosStartTime;
        //public DateTime dosEndTime;

        public List<clsTriageItem> listOfTriageItems;
    }
}
