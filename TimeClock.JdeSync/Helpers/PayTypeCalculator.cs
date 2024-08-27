using Serilog;

namespace TimeClock.JdeSync.Helpers
{
    internal class PayTypeCalculator
    {
        public PayTypeCalculator(ILogger logger)
        {

        }
    }

    internal class PayTypeConfiguration
    {
        public int StraightTimeDba {  get; set; }
        public int OvertimeDba {  get; set; }
        public string StraightTimeGlCode { get; set; } = null!;
        public string OvertimeGlCode { get; set; } = null!;
        public string CompanyId { get; set;} = null!;
        public PayTypeConfiguration()
        {
            this.DefaultOvertimeRule = new DefaultOvertimeRule()
            {
                AfterHours = 40m, OvertimeThreshold = OvertimeThresholdType.Week, 
                SpecialPay = new SpecialPay() 
                { 
                    Holliday = new IncreaseRate() { Amount = 1.5m, IncreaseRateType = IncreaseRateType.Percentage },
                    Saturday = new IncreaseRate() { Amount = 2m, IncreaseRateType = IncreaseRateType.FlatIncreaase },
                    Sunday = new IncreaseRate() { Amount = 50m, IncreaseRateType = IncreaseRateType.FlatPay }
                }
            };
            this.SpecialOvertimeRules = [
                new SpecialOvertimeRule() { AfterHours = 8m, Objects = "JN;UUU", ObjectsType = SpecialOvertimeObjectType.Unions, OvertimeThreshold = OvertimeThresholdType.Day, 
                    SpecialPay = new SpecialPay() { 
                        Holliday = new IncreaseRate() { Amount = 2.5m, IncreaseRateType = IncreaseRateType.Percentage },
                        Saturday = new IncreaseRate(){ Amount = 10, IncreaseRateType = IncreaseRateType.FlatIncreaase },
                        Sunday = new IncreaseRate() { Amount = 2m, IncreaseRateType = IncreaseRateType.Percentage }
                    } 
                },
                new SpecialOvertimeRule() { AfterHours = 6m, Objects = "UOA", ObjectsType = SpecialOvertimeObjectType.Unions, OvertimeThreshold = OvertimeThresholdType.Day,
                    SpecialPay = new SpecialPay() {
                        Holliday = new IncreaseRate() { Amount = 3m, IncreaseRateType = IncreaseRateType.Percentage },
                    }
                },
                new SpecialOvertimeRule() { AfterHours = 6m, Objects = "6001;6005;2014", ObjectsType = SpecialOvertimeObjectType.Projects, OvertimeThreshold = OvertimeThresholdType.Week,
                    SpecialPay = new SpecialPay() {
                        Holliday = new IncreaseRate() { Amount = 15m, IncreaseRateType = IncreaseRateType.FlatIncreaase },
                        Saturday = new IncreaseRate() { Amount = 15m, IncreaseRateType = IncreaseRateType.FlatIncreaase },
                        Sunday = new IncreaseRate() { Amount = 15m, IncreaseRateType = IncreaseRateType.FlatIncreaase },
                    }
                }
                ];
            this.JobTypePayRules = [
                new JobTypePayRule() { JobTypes = "54;88;76;98", IncreaseRate = new IncreaseRate() { Amount = 5m, IncreaseRateType = IncreaseRateType.FlatIncreaase }, SpecialPay = new SpecialPay(){
                    Holliday = new IncreaseRate() { Amount = 1.5m, IncreaseRateType = IncreaseRateType.Percentage },
                    Saturday = new IncreaseRate() { Amount = 8m, IncreaseRateType = IncreaseRateType.FlatIncreaase },
                    Sunday = new IncreaseRate() { Amount = 10, IncreaseRateType = IncreaseRateType.FlatIncreaase }
                }},new JobTypePayRule() { JobTypes = "22;54;12;33;45;78;99", IncreaseRate = new IncreaseRate() { Amount = 2m, IncreaseRateType = IncreaseRateType.FlatIncreaase }, SpecialPay = new SpecialPay(){
                    Holliday = new IncreaseRate() { Amount = 1.25m, IncreaseRateType = IncreaseRateType.Percentage },
                    Saturday = new IncreaseRate() { Amount = 3m, IncreaseRateType = IncreaseRateType.FlatIncreaase },
                    Sunday = new IncreaseRate() { Amount = 5, IncreaseRateType = IncreaseRateType.FlatIncreaase }
                }},
                ];
        }
        /// <summary>
        /// Required
        /// </summary>
        public DefaultOvertimeRule DefaultOvertimeRule { get; set; } = null!;
        /// <summary>
        /// Optional
        /// </summary>
        public SpecialOvertimeRule[] SpecialOvertimeRules { get; set; } = [];
        /// <summary>
        /// Optional
        /// </summary>
        public JobTypePayRule[] JobTypePayRules { get; set; } = [];
    }

    internal class DefaultOvertimeRule
    {

        /// <summary>
        /// Number of hours to threshold after which overtime will begin
        /// </summary>
        public decimal AfterHours { get; set; }
        public OvertimeThresholdType OvertimeThreshold { get; set; }
        /// <summary>
        /// Optional
        /// </summary>
        public SpecialPay? SpecialPay { get; set; }
    }

    internal class SpecialOvertimeRule
    {
        private IList<string>? _objectsCollection;
        /// <summary>
        /// List of semi-colon seperated projects or unions
        /// </summary>
        public string Objects { get; set; } = null!;
        /// <summary>
        /// Number of hours to threshold after which overtime will begin
        /// </summary>
        public decimal AfterHours { get; set; }
        public OvertimeThresholdType OvertimeThreshold { get; set; }
        /// <summary>
        /// Optional
        /// </summary>
        public SpecialPay? SpecialPay { get; set; }
        public SpecialOvertimeObjectType ObjectsType { get; set; }
        public IList<string> GetObjectsAsCollection()
        {
            if (this._objectsCollection is null)
                this._objectsCollection = this.Objects.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return this._objectsCollection;
        }
    }

    internal class JobTypePayRule
    {
        private IList<string>? _objectsCollection;
        /// <summary>
        /// List of semi-colon seperated job codes
        /// </summary>
        public string JobTypes { get; set; } = null!;
        public IncreaseRate IncreaseRate { get; set; } = null!;
        /// <summary>
        /// Optional
        /// </summary>
        public SpecialPay? SpecialPay { get; set; }
        public IList<string> GetJobTypesAsCollection()
        {
            if (this._objectsCollection is null)
                this._objectsCollection = this.JobTypes.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return this._objectsCollection;
        }
    }

    internal class SpecialPay
    {
        /// <summary>
        /// Optional
        /// </summary>
        public IncreaseRate? Holliday { get; set; }
        /// <summary>
        /// Optional
        /// </summary>
        public IncreaseRate? Saturday { get; set; }
        /// <summary>
        /// Optional
        /// </summary>
        public IncreaseRate? Sunday { get; set; }
    }

    internal class IncreaseRate
    {
        public decimal Amount { get; set; }
        public IncreaseRateType IncreaseRateType { get; set; }
    }

    public enum OvertimeThresholdType
    {
        Day,
        Week
    }

    internal enum SpecialOvertimeObjectType
    {
        Unions,
        Projects
    }

    internal enum IncreaseRateType
    {
        Percentage,
        FlatIncreaase,
        FlatPay
    }
}
