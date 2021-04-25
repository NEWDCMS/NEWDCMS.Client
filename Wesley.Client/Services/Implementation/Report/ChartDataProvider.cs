using Wesley.Easycharts;
using SkiaSharp;
using System.Collections.Generic;
using System.Diagnostics;

namespace Wesley.Client.Services
{
    /// <summary>
    /// 用于图表数据提供
    /// </summary>
    public static class ChartDataProvider
    {
        public static readonly SKColor[] Colors = {
            SKColor.Parse("#266489"),
            SKColor.Parse("#68B9C0"),
            SKColor.Parse("#90D585"),
            SKColor.Parse("#F3C151"),
            SKColor.Parse("#F37F64"),
            SKColor.Parse("#424856"),
            SKColor.Parse("#8F97A4"),
            SKColor.Parse("#DAC096"),
            SKColor.Parse("#76846E"),
            SKColor.Parse("#DABFAF"),
            SKColor.Parse("#A65B69"),
            SKColor.Parse("#97A69D"),
            SKColor.Parse("#266489"),
            SKColor.Parse("#68B9C0"),
            SKColor.Parse("#90D585"),
            SKColor.Parse("#F3C151"),
            SKColor.Parse("#F37F64"),
            SKColor.Parse("#424856"),
            SKColor.Parse("#8F97A4"),
            SKColor.Parse("#DAC096"),
            SKColor.Parse("#76846E"),
            SKColor.Parse("#DABFAF"),
            SKColor.Parse("#A65B69"),
            SKColor.Parse("#97A69D")
        };

        #region Demo 测试

#if DEBUG
        public static ChartEntry[] Entries
        {
            get
            {
                var entries = new[]
                {
                    new ChartEntry(212254.45f)
                    {
                        Label = "雪花勇闯天涯",
                        ValueLabel = "212,254.45",
                        Color = SKColor.Parse("#2c3e50")
                    },
                    new ChartEntry(248254.45f)
                    {
                        Label = "青岛九度",
                        ValueLabel = "248,254.45",
                        Color = SKColor.Parse("#77d065")
                    },
                    new ChartEntry(128254.45f)
                    {
                        Label = "雪花匠心营造",
                        ValueLabel = "128,254.45",
                        Color = SKColor.Parse("#b455b6")
                    },
                    new ChartEntry(514254.45f)
                    {
                        Label = "雪花脸谱",
                        ValueLabel = "514,254.45",
                        Color = SKColor.Parse("#3498db")
                    },
                    new ChartEntry(212254.45f)
                    {
                        Label = "雪花马尔斯绿",
                        ValueLabel = "212,254.45",
                        Color = SKColor.Parse("#2c3e50")
                    },
                    new ChartEntry(222254.45f)
                    {
                        Label = "青岛纯生啤酒",
                        ValueLabel = "222,254.45",
                        Color = SKColor.Parse("#77d065")
                    }
                };
                return entries;
            }
        }

        public static ChartEntry[] Entries2
        {
            get
            {
                var entries = new[]
                {
                    new ChartEntry(212254.45f)
                    {
                        Label = "雪花勇闯天涯",
                        ValueLabel = "212,254.45",
                        Color = SKColor.Parse("#2c3e50")
                    },
                    new ChartEntry(248254.45f)
                    {
                        Label = "青岛九度",
                        ValueLabel = "248,254.45",
                        Color = SKColor.Parse("#77d065")
                    },
                    new ChartEntry(128254.45f)
                    {
                        Label = "雪花匠心营造",
                        ValueLabel = "128,254.45",
                        Color = SKColor.Parse("#b455b6")
                    },
                    new ChartEntry(514254.45f)
                    {
                        Label = "雪花脸谱",
                        ValueLabel = "514,254.45",
                        Color = SKColor.Parse("#3498db")
                    },new ChartEntry(212254.45f)
                    {
                        Label = "雪花马尔斯绿",
                        ValueLabel = "212,254.45",
                        Color = SKColor.Parse("#2c3e50")
                    },
                    new ChartEntry(222254.45f)
                    {
                        Label = "青岛纯生啤酒",
                        ValueLabel = "222,254.45",
                        Color = SKColor.Parse("#77d065")
                    },
                    new ChartEntry(678254.45f)
                    {
                        Label = "喜力啤酒",
                        ValueLabel = "678,254.45",
                        Color = SKColor.Parse("#b455b6")
                    },
                    new ChartEntry(934254.45f)
                    {
                        Label = "金威",
                        ValueLabel = "934,254.45",
                        Color = SKColor.Parse("#3498db")
                    }
                };
                return entries;
            }
        }
#elif RELEASE
		public static ChartEntry[] Entries
		{
			get
			{
				var entries = new[]
				{
					new ChartEntry(212254.45f)
					{
						Label = "雪花勇闯天涯",
						ValueLabel = "212,254.45",
						Color = SKColor.Parse("#2c3e50")
					},
					new ChartEntry(248254.45f)
					{
						Label = "青岛九度",
						ValueLabel = "248,254.45",
						Color = SKColor.Parse("#77d065")
					},
					new ChartEntry(128254.45f)
					{
						Label = "雪花匠心营造",
						ValueLabel = "128,254.45",
						Color = SKColor.Parse("#b455b6")
					},
					new ChartEntry(514254.45f)
					{
						Label = "雪花脸谱",
						ValueLabel = "514,254.45",
						Color = SKColor.Parse("#3498db")
					},
					new ChartEntry(212254.45f)
					{
						Label = "雪花马尔斯绿",
						ValueLabel = "212,254.45",
						Color = SKColor.Parse("#2c3e50")
					},
					new ChartEntry(222254.45f)
					{
						Label = "青岛纯生啤酒",
						ValueLabel = "222,254.45",
						Color = SKColor.Parse("#77d065")
					}
				};
				return entries;
			}
		}

		public static ChartEntry[] Entries2
		{
			get
			{
				var entries = new[]
				{
					new ChartEntry(212254.45f)
					{
						Label = "雪花勇闯天涯",
						ValueLabel = "212,254.45",
						Color = SKColor.Parse("#2c3e50")
					},
					new ChartEntry(248254.45f)
					{
						Label = "青岛九度",
						ValueLabel = "248,254.45",
						Color = SKColor.Parse("#77d065")
					},
					new ChartEntry(128254.45f)
					{
						Label = "雪花匠心营造",
						ValueLabel = "128,254.45",
						Color = SKColor.Parse("#b455b6")
					},
					new ChartEntry(514254.45f)
					{
						Label = "雪花脸谱",
						ValueLabel = "514,254.45",
						Color = SKColor.Parse("#3498db")
					},new ChartEntry(212254.45f)
					{
						Label = "雪花马尔斯绿",
						ValueLabel = "212,254.45",
						Color = SKColor.Parse("#2c3e50")
					},
					new ChartEntry(222254.45f)
					{
						Label = "青岛纯生啤酒",
						ValueLabel = "222,254.45",
						Color = SKColor.Parse("#77d065")
					},
					new ChartEntry(678254.45f)
					{
						Label = "喜力啤酒",
						ValueLabel = "678,254.45",
						Color = SKColor.Parse("#b455b6")
					},
					new ChartEntry(934254.45f)
					{
						Label = "金威",
						ValueLabel = "934,254.45",
						Color = SKColor.Parse("#3498db")
					}
				};
				return entries;
			}
		}
#endif

        /// <summary>
        /// HorizontalBarChart
        /// </summary>
        /// <returns></returns>
        public static Chart CreateHorizontalBarChart(List<ChartEntry> entries)
        {
            var fontManager = SKFontManager.Default;
            var typeface = fontManager.MatchCharacter('雪');

            return new HorizontalBarChart()
            {
                Entries = entries,
                LabelTextSize = 30,
                Typeface = typeface,
            };
        }


        /// <summary>
        /// BarChart
        /// </summary>
        /// <returns></returns>
        public static Chart CreateBarChart(List<ChartEntry> entries)
        {
            var fontManager = SKFontManager.Default;
            var typeface = fontManager.MatchCharacter('雪');

            return new BarChart()
            {
                Entries = entries,
                Typeface = typeface,
                LabelTextSize = 30,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelOrientation = Orientation.Rotate
            };
        }

        /// <summary>
        /// PointChart
        /// </summary>
        /// <returns></returns>
        public static Chart CreatePointChart(List<ChartEntry> entries)
        {
            var fontManager = SKFontManager.Default;
            var typeface = fontManager.MatchCharacter('雪');
            return new PointChart()
            {
                Entries = entries,
                Typeface = typeface,
                LabelTextSize = 30,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelOrientation = Orientation.Horizontal
            };
        }


        /// <summary>
        /// LineChart
        /// </summary>
        /// <returns></returns>
        public static Chart CreateLineChart(List<ChartEntry> entries)
        {
            var fontManager = SKFontManager.Default;
            var typeface = fontManager.MatchCharacter('雪');

            Debug.WriteLine($"{ entries.Count}");
            return new LineChart()
            {
                Entries = entries,
                Typeface = typeface,
                LineMode = LineMode.Straight,
                LineSize = 2,
                PointMode = PointMode.Circle,
                PointSize = 18,
                LabelTextSize = 30,

                LabelOrientation = Orientation.Vertical
            };
        }

        /// <summary>
        /// DonutChart
        /// </summary>
        /// <returns></returns>
        public static Chart CreateDonutChart(List<ChartEntry> entries)
        {
            var fontManager = SKFontManager.Default;
            var typeface = fontManager.MatchCharacter('雪');
            return new DonutChart()
            {
                Entries = entries,
                Typeface = typeface,
                LabelTextSize = 30
            };
        }

        /// <summary>
        /// RadialGaugeChart
        /// </summary>
        /// <returns></returns>
        public static Chart CreateRadialGaugeChart(List<ChartEntry> entries)
        {
            var fontManager = SKFontManager.Default;
            var typeface = fontManager.MatchCharacter('雪');
            return new RadialGaugeChart()
            {
                Entries = entries,
                Typeface = typeface,
                LabelTextSize = 30
            };
        }

        /// <summary>
        /// RadarChart
        /// </summary>
        /// <returns></returns>
        public static Chart CreateRadarChart(List<ChartEntry> entries)
        {
            var fontManager = SKFontManager.Default;
            var typeface = fontManager.MatchCharacter('雪');
            return new RadarChart()
            {
                Entries = entries,
                Typeface = typeface,
                LabelTextSize = 30
            };
        }


        #endregion
    }
}
