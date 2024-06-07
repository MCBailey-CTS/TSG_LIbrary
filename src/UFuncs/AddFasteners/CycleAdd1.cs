using System.Collections.Generic;

namespace TSG_Library.UFuncs
{
    public struct CycleAdd1
    {
        /// <summary>
        /// </summary>
        // ReSharper disable once NotAccessedField.Global
        public static double MetricCycle;

        /// <summary>
        /// </summary>
        // ReSharper disable once NotAccessedField.Global
        public static double EnglishCycle;

        /// <summary>
        /// </summary>
        // ReSharper disable once NotAccessedField.Global
        public double Diameter;

        /// <summary>
        /// </summary>
        // ReSharper disable once NotAccessedField.Global
        public string ShortDowel;

        /// <summary>
        /// </summary>
        // ReSharper disable once NotAccessedField.Global
        public string LongDowel;

        /// <summary>
        /// </summary>
        // ReSharper disable once NotAccessedField.Global
        public string JackCycle;

        /// <summary>
        /// </summary>
        /// <param name="diameter"></param>
        /// <param name="shortDowel"></param>
        /// <param name="longDowel"></param>
        /// <param name="jackCycle"></param>
        public CycleAdd1(double diameter, string shortDowel, string longDowel, string jackCycle)
        {
            Diameter = diameter;
            ShortDowel = shortDowel;
            LongDowel = longDowel;
            JackCycle = jackCycle;
        }

        public static int MetricDelimeter = 30;

        public static int EnglishDelimeter = 125;

        public static IDictionary<string, string[]> MetricCyclePair
        {
            get
            {
                return new Dictionary<string, string[]>
                {
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\004",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-020.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\\6mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\005",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-020.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\6mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\006",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-020.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\6mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\008",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\8mm-dwl-025.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\8mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\8mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\010",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\10mm-dwl-025.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\10mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\10mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\012",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\12mm-dwl-025.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\12mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\016",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\16mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\16mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\020",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\20mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\20mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\024",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\030",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\036",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\0006",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\0250-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\0250-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\_0250-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\0008",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\0250-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\0250-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\_0250-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\0010",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0250-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0250-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0250-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\0250",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0250-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0250-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0250-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\0313",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0313-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0313-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0313-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\0375",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0375-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0375-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0375-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\0500",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0500-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0500-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\0625",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0625-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0625-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\0750",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0750-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0750-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\0875",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0750-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0750-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\1000",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\1250",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\1500",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },


                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\004-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-020.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\\6mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\005-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-020.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\6mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\006-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-020.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\6mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\008-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\8mm-dwl-025.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\8mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\8mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\010-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\10mm-dwl-025.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\10mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\10mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\012-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\12mm-dwl-025.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\12mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\016-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\16mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\16mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\020-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\20mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\20mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\024-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\030-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\036-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\0006-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\0250-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\0250-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\_0250-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\0008-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\0250-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\0250-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\_0250-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\0010-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0250-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0250-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0250-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\0250-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0250-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0250-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0250-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\0313-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0313-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0313-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0313-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\0375-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0375-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0375-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0375-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\0500-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0500-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0500-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\0625-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0625-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0625-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\0750-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0750-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0750-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\0875-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0750-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0750-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\1000-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\1250-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\1500-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    }
                };
            }
        }
    }
}