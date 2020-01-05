# Azure Function App with Https and Event Hub Trigger

## Scope

* Function to use dotnet core C#
* Single code base with two different triggers: https and event-hub
* The function will be depoyed twice with the same code base, but two different triggers

### Https Trigger

* [Spec](https://app.swaggerhub.com/apis/vkhazin/trgos-faretypes/1.0.0)
* Https End-Point to execute write (insert/update/delete) requests against Sql Server 2019
* Sql Server DDL:
```
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FareTypes](
	[FareTypeId] [int] NOT NULL,
	[FareTypeAbbr] [varchar](10) NULL,
	[Description] [varchar](30) NULL,
	[FlatFareAmt] [real] NULL,
	[FlatRate] [real] NULL,
	[DistanceRate] [real] NULL,
	[PolygonRate] [real] NULL,
	[TimeRate] [real] NULL,
	[PolyTypeId] [int] NULL,
	[PrepaidReq] [char](2) NULL,
	[FareAdjustDiffZon] [float] NULL,
	[FareAdjustSameZon] [float] NULL,
	[DistanceLimitSameZon] [float] NULL,
	[DistanceLimitDiffZon] [float] NULL,
	[NumCode] [smallint] NULL,
	[FareMode] [smallint] NULL,
	[FareCalcType] [smallint] NULL,
	[VariationOf] [int] NULL,
	[MinFare] [real] NULL,
	[MaxFare] [real] NULL,
	[RoundFare] [tinyint] NULL,
	[DistanceLimit] [int] NULL,
	[RoundDirection] [smallint] NULL,
	[Accuracy] [real] NULL,
	[InActive] [smallint] NULL,
	[Timestamp] [datetime2] DEFAULT 50SYSUTCDATETIME ( )
 CONSTRAINT [pkFareTypes] PRIMARY KEY CLUSTERED 
(
	[FareTypeId] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
```
* Sql Server sample data:
```
GO
INSERT [dbo].[FareTypes] ([FareTypeId], [FareTypeAbbr], [Description], [FlatFareAmt], [FlatRate], [DistanceRate], [PolygonRate], [TimeRate], [PolyTypeId], [PrepaidReq], [FareAdjustDiffZon], [FareAdjustSameZon], [DistanceLimitSameZon], [DistanceLimitDiffZon], [NumCode], [FareMode], [FareCalcType], [VariationOf], [MinFare], [MaxFare], [RoundFare], [DistanceLimit], [RoundDirection], [Accuracy], [InActive]) VALUES (1, N'STA', N'Standard Fare', NULL, 2, 0, 0, 0, 0, NULL, 0, 0, 0, 0, 1, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[FareTypes] ([FareTypeId], [FareTypeAbbr], [Description], [FlatFareAmt], [FlatRate], [DistanceRate], [PolygonRate], [TimeRate], [PolyTypeId], [PrepaidReq], [FareAdjustDiffZon], [FareAdjustSameZon], [DistanceLimitSameZon], [DistanceLimitDiffZon], [NumCode], [FareMode], [FareCalcType], [VariationOf], [MinFare], [MaxFare], [RoundFare], [DistanceLimit], [RoundDirection], [Accuracy], [InActive]) VALUES (2, N'CON', N'Concessionary Fare', NULL, 1, 0, 0, 0, 0, NULL, 0, 0, 0, 0, 2, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[FareTypes] ([FareTypeId], [FareTypeAbbr], [Description], [FlatFareAmt], [FlatRate], [DistanceRate], [PolygonRate], [TimeRate], [PolyTypeId], [PrepaidReq], [FareAdjustDiffZon], [FareAdjustSameZon], [DistanceLimitSameZon], [DistanceLimitDiffZon], [NumCode], [FareMode], [FareCalcType], [VariationOf], [MinFare], [MaxFare], [RoundFare], [DistanceLimit], [RoundDirection], [Accuracy], [InActive]) VALUES (3, N'CSF', N'Child Standard Fare', NULL, 1, 0, 0, 0, 0, NULL, 0, 0, 0, 0, 3, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[FareTypes] ([FareTypeId], [FareTypeAbbr], [Description], [FlatFareAmt], [FlatRate], [DistanceRate], [PolygonRate], [TimeRate], [PolyTypeId], [PrepaidReq], [FareAdjustDiffZon], [FareAdjustSameZon], [DistanceLimitSameZon], [DistanceLimitDiffZon], [NumCode], [FareMode], [FareCalcType], [VariationOf], [MinFare], [MaxFare], [RoundFare], [DistanceLimit], [RoundDirection], [Accuracy], [InActive]) VALUES (4, N'CCF', N'Child Concessionary Fare', NULL, 0.5, 0, 0, 0, 0, NULL, 0, 0, 0, 0, 4, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[FareTypes] ([FareTypeId], [FareTypeAbbr], [Description], [FlatFareAmt], [FlatRate], [DistanceRate], [PolygonRate], [TimeRate], [PolyTypeId], [PrepaidReq], [FareAdjustDiffZon], [FareAdjustSameZon], [DistanceLimitSameZon], [DistanceLimitDiffZon], [NumCode], [FareMode], [FareCalcType], [VariationOf], [MinFare], [MaxFare], [RoundFare], [DistanceLimit], [RoundDirection], [Accuracy], [InActive]) VALUES (5, N'ESC', N'Escort Fare', NULL, 0.75, 0, 0, 0, 0, NULL, 0, 0, 0, 0, 5, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[FareTypes] ([FareTypeId], [FareTypeAbbr], [Description], [FlatFareAmt], [FlatRate], [DistanceRate], [PolygonRate], [TimeRate], [PolyTypeId], [PrepaidReq], [FareAdjustDiffZon], [FareAdjustSameZon], [DistanceLimitSameZon], [DistanceLimitDiffZon], [NumCode], [FareMode], [FareCalcType], [VariationOf], [MinFare], [MaxFare], [RoundFare], [DistanceLimit], [RoundDirection], [Accuracy], [InActive]) VALUES (6, N'FOC', N'Free Of Charge', NULL, 0, 0, 0, 0, 0, NULL, 0, 0, 0, 0, 6, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
```
* Https end-point to execute read (select) requests against Cosmos DB
* Cosmos DB sample data identical to Sql Server plus `SyncTimestamp` datetime property, e.g.:
```
{
	"FareTypeId": 1,
	"FareTypeAbbr": "STA",
	"Description": "Standard Fare",
	"FlatRate": 2.0,
	"DistanceRate": 0.0,
	"PolygonRate": 0.0,
	"TimeRate": 0.0,
	"PolyTypeId": 0,
	"FareAdjustDiffZon": 0.0,
	"FareAdjustSameZon": 0.0,
	"DistanceLimitSameZon": 0.0,
	"DistanceLimitDiffZon": 0.0,
	"NumCode": 1,
	"FareMode": 0,
	"FareCalcType": 0,
	"SourceTimestamp": "2002-10-02T15:00:00.05Z",
	"SyncTimestamp": "2002-10-02T15:00:00.05Z"
}
```

### Event Hub Trigger

* Message format:
```
{
  "table": "FareTypes",
  "action": "insert|update|delete",
  "timestamp": "2002-10-02T15:00:00.05Z",
  "id": "single field pk field value",
  "data": {
	  "FareTypeId": 1,
	  "FareTypeAbbr": "STA",
	  "Description": "Standard Fare",
	  "FlatRate": 2.0,
	  "DistanceRate": 0.0,
	  "PolygonRate": 0.0,
	  "TimeRate": 0.0,
	  "PolyTypeId": 0,
	  "FareAdjustDiffZon": 0.0,
	  "FareAdjustSameZon": 0.0,
	  "DistanceLimitSameZon": 0.0,
	  "DistanceLimitDiffZon": 0.0,
	  "NumCode": 1,
	  "FareMode": 0,
	  "FareCalcType": 0,
	  "SourceTimestamp": "2002-10-02T15:00:00.05Z"
  }
}
```
* Based on the action insert, update, or delete a record in the Cosmos DB
* Update `SyncTimestamp` when writing to Cosmos DB

