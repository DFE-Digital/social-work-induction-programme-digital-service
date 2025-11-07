using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Mappings;

public class LocalAuthorityMapping : IEntityTypeConfiguration<LocalAuthority>
{
    public void Configure(EntityTypeBuilder<LocalAuthority> builder)
    {
        builder.ToTable("local_authorities");
        builder.HasKey(la => la.OldLaCode);
        builder.Property(la => la.OldLaCode).IsRequired();
        builder.Property(la => la.RegionCode).HasMaxLength(15);
        builder.Property(la => la.RegionName).HasMaxLength(100).IsRequired();
        builder.Property(la => la.LaName).HasMaxLength(100).IsRequired();
        builder.Property(la => la.NewLaCode).HasMaxLength(15);

        builder.HasData(
            new LocalAuthority
            {
                OldLaCode = 840,
                RegionCode = "E12000001",
                RegionName = "North East",
                LaName = "County Durham",
                NewLaCode = "E06000047",
            },
            new LocalAuthority
            {
                OldLaCode = 841,
                RegionCode = "E12000001",
                RegionName = "North East",
                LaName = "Darlington",
                NewLaCode = "E06000005",
            },
            new LocalAuthority
            {
                OldLaCode = 390,
                RegionCode = "E12000001",
                RegionName = "North East",
                LaName = "Gateshead",
                NewLaCode = "E08000037",
            },
            new LocalAuthority
            {
                OldLaCode = 805,
                RegionCode = "E12000001",
                RegionName = "North East",
                LaName = "Hartlepool",
                NewLaCode = "E06000001",
            },
            new LocalAuthority
            {
                OldLaCode = 806,
                RegionCode = "E12000001",
                RegionName = "North East",
                LaName = "Middlesbrough",
                NewLaCode = "E06000002",
            },
            new LocalAuthority
            {
                OldLaCode = 391,
                RegionCode = "E12000001",
                RegionName = "North East",
                LaName = "Newcastle upon Tyne",
                NewLaCode = "E08000021",
            },
            new LocalAuthority
            {
                OldLaCode = 392,
                RegionCode = "E12000001",
                RegionName = "North East",
                LaName = "North Tyneside",
                NewLaCode = "E08000022",
            },
            new LocalAuthority
            {
                OldLaCode = 929,
                RegionCode = "E12000001",
                RegionName = "North East",
                LaName = "Northumberland",
                NewLaCode = "E06000057",
            },
            new LocalAuthority
            {
                OldLaCode = 807,
                RegionCode = "E12000001",
                RegionName = "North East",
                LaName = "Redcar and Cleveland",
                NewLaCode = "E06000003",
            },
            new LocalAuthority
            {
                OldLaCode = 393,
                RegionCode = "E12000001",
                RegionName = "North East",
                LaName = "South Tyneside",
                NewLaCode = "E08000023",
            },
            new LocalAuthority
            {
                OldLaCode = 808,
                RegionCode = "E12000001",
                RegionName = "North East",
                LaName = "Stockton-on-Tees",
                NewLaCode = "E06000004",
            },
            new LocalAuthority
            {
                OldLaCode = 394,
                RegionCode = "E12000001",
                RegionName = "North East",
                LaName = "Sunderland",
                NewLaCode = "E08000024",
            },
            new LocalAuthority
            {
                OldLaCode = 889,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Blackburn with Darwen",
                NewLaCode = "E06000008",
            },
            new LocalAuthority
            {
                OldLaCode = 890,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Blackpool",
                NewLaCode = "E06000009",
            },
            new LocalAuthority
            {
                OldLaCode = 350,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Bolton",
                NewLaCode = "E08000001",
            },
            new LocalAuthority
            {
                OldLaCode = 351,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Bury",
                NewLaCode = "E08000002",
            },
            new LocalAuthority
            {
                OldLaCode = 895,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Cheshire East",
                NewLaCode = "E06000049",
            },
            new LocalAuthority
            {
                OldLaCode = 896,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Cheshire West and Chester",
                NewLaCode = "E06000050",
            },
            new LocalAuthority
            {
                OldLaCode = 942,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Cumberland",
                NewLaCode = "E06000063",
            },
            new LocalAuthority
            {
                OldLaCode = 943,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Westmorland and Furness",
                NewLaCode = "E06000064",
            },
            new LocalAuthority
            {
                OldLaCode = 876,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Halton",
                NewLaCode = "E06000006",
            },
            new LocalAuthority
            {
                OldLaCode = 340,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Knowsley",
                NewLaCode = "E08000011",
            },
            new LocalAuthority
            {
                OldLaCode = 888,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Lancashire",
                NewLaCode = "E10000017",
            },
            new LocalAuthority
            {
                OldLaCode = 341,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Liverpool",
                NewLaCode = "E08000012",
            },
            new LocalAuthority
            {
                OldLaCode = 352,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Manchester",
                NewLaCode = "E08000003",
            },
            new LocalAuthority
            {
                OldLaCode = 353,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Oldham",
                NewLaCode = "E08000004",
            },
            new LocalAuthority
            {
                OldLaCode = 354,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Rochdale",
                NewLaCode = "E08000005",
            },
            new LocalAuthority
            {
                OldLaCode = 355,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Salford",
                NewLaCode = "E08000006",
            },
            new LocalAuthority
            {
                OldLaCode = 343,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Sefton",
                NewLaCode = "E08000014",
            },
            new LocalAuthority
            {
                OldLaCode = 342,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "St. Helens",
                NewLaCode = "E08000013",
            },
            new LocalAuthority
            {
                OldLaCode = 356,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Stockport",
                NewLaCode = "E08000007",
            },
            new LocalAuthority
            {
                OldLaCode = 357,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Tameside",
                NewLaCode = "E08000008",
            },
            new LocalAuthority
            {
                OldLaCode = 358,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Trafford",
                NewLaCode = "E08000009",
            },
            new LocalAuthority
            {
                OldLaCode = 877,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Warrington",
                NewLaCode = "E06000007",
            },
            new LocalAuthority
            {
                OldLaCode = 359,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Wigan",
                NewLaCode = "E08000010",
            },
            new LocalAuthority
            {
                OldLaCode = 344,
                RegionCode = "E12000002",
                RegionName = "North West",
                LaName = "Wirral",
                NewLaCode = "E08000015",
            },
            new LocalAuthority
            {
                OldLaCode = 370,
                RegionCode = "E12000003",
                RegionName = "Yorkshire and The Humber",
                LaName = "Barnsley",
                NewLaCode = "E08000016",
            },
            new LocalAuthority
            {
                OldLaCode = 380,
                RegionCode = "E12000003",
                RegionName = "Yorkshire and The Humber",
                LaName = "Bradford",
                NewLaCode = "E08000032",
            },
            new LocalAuthority
            {
                OldLaCode = 381,
                RegionCode = "E12000003",
                RegionName = "Yorkshire and The Humber",
                LaName = "Calderdale",
                NewLaCode = "E08000033",
            },
            new LocalAuthority
            {
                OldLaCode = 371,
                RegionCode = "E12000003",
                RegionName = "Yorkshire and The Humber",
                LaName = "Doncaster",
                NewLaCode = "E08000017",
            },
            new LocalAuthority
            {
                OldLaCode = 811,
                RegionCode = "E12000003",
                RegionName = "Yorkshire and The Humber",
                LaName = "East Riding of Yorkshire",
                NewLaCode = "E06000011",
            },
            new LocalAuthority
            {
                OldLaCode = 810,
                RegionCode = "E12000003",
                RegionName = "Yorkshire and The Humber",
                LaName = "Kingston upon Hull, City of",
                NewLaCode = "E06000010",
            },
            new LocalAuthority
            {
                OldLaCode = 382,
                RegionCode = "E12000003",
                RegionName = "Yorkshire and The Humber",
                LaName = "Kirklees",
                NewLaCode = "E08000034",
            },
            new LocalAuthority
            {
                OldLaCode = 383,
                RegionCode = "E12000003",
                RegionName = "Yorkshire and The Humber",
                LaName = "Leeds",
                NewLaCode = "E08000035",
            },
            new LocalAuthority
            {
                OldLaCode = 812,
                RegionCode = "E12000003",
                RegionName = "Yorkshire and The Humber",
                LaName = "North East Lincolnshire",
                NewLaCode = "E06000012",
            },
            new LocalAuthority
            {
                OldLaCode = 813,
                RegionCode = "E12000003",
                RegionName = "Yorkshire and The Humber",
                LaName = "North Lincolnshire",
                NewLaCode = "E06000013",
            },
            new LocalAuthority
            {
                OldLaCode = 815,
                RegionCode = "E12000003",
                RegionName = "Yorkshire and The Humber",
                LaName = "North Yorkshire",
                NewLaCode = "E06000065",
            },
            new LocalAuthority
            {
                OldLaCode = 372,
                RegionCode = "E12000003",
                RegionName = "Yorkshire and The Humber",
                LaName = "Rotherham",
                NewLaCode = "E08000018",
            },
            new LocalAuthority
            {
                OldLaCode = 373,
                RegionCode = "E12000003",
                RegionName = "Yorkshire and The Humber",
                LaName = "Sheffield",
                NewLaCode = "E08000019",
            },
            new LocalAuthority
            {
                OldLaCode = 384,
                RegionCode = "E12000003",
                RegionName = "Yorkshire and The Humber",
                LaName = "Wakefield",
                NewLaCode = "E08000036",
            },
            new LocalAuthority
            {
                OldLaCode = 816,
                RegionCode = "E12000003",
                RegionName = "Yorkshire and The Humber",
                LaName = "York",
                NewLaCode = "E06000014",
            },
            new LocalAuthority
            {
                OldLaCode = 831,
                RegionCode = "E12000004",
                RegionName = "East Midlands",
                LaName = "Derby",
                NewLaCode = "E06000015",
            },
            new LocalAuthority
            {
                OldLaCode = 830,
                RegionCode = "E12000004",
                RegionName = "East Midlands",
                LaName = "Derbyshire",
                NewLaCode = "E10000007",
            },
            new LocalAuthority
            {
                OldLaCode = 856,
                RegionCode = "E12000004",
                RegionName = "East Midlands",
                LaName = "Leicester",
                NewLaCode = "E06000016",
            },
            new LocalAuthority
            {
                OldLaCode = 855,
                RegionCode = "E12000004",
                RegionName = "East Midlands",
                LaName = "Leicestershire",
                NewLaCode = "E10000018",
            },
            new LocalAuthority
            {
                OldLaCode = 925,
                RegionCode = "E12000004",
                RegionName = "East Midlands",
                LaName = "Lincolnshire",
                NewLaCode = "E10000019",
            },
            new LocalAuthority
            {
                OldLaCode = 940,
                RegionCode = "E12000004",
                RegionName = "East Midlands",
                LaName = "North Northamptonshire",
                NewLaCode = "E06000061",
            },
            new LocalAuthority
            {
                OldLaCode = 892,
                RegionCode = "E12000004",
                RegionName = "East Midlands",
                LaName = "Nottingham",
                NewLaCode = "E06000018",
            },
            new LocalAuthority
            {
                OldLaCode = 891,
                RegionCode = "E12000004",
                RegionName = "East Midlands",
                LaName = "Nottinghamshire",
                NewLaCode = "E10000024",
            },
            new LocalAuthority
            {
                OldLaCode = 857,
                RegionCode = "E12000004",
                RegionName = "East Midlands",
                LaName = "Rutland",
                NewLaCode = "E06000017",
            },
            new LocalAuthority
            {
                OldLaCode = 941,
                RegionCode = "E12000004",
                RegionName = "East Midlands",
                LaName = "West Northamptonshire",
                NewLaCode = "E06000062",
            },
            new LocalAuthority
            {
                OldLaCode = 330,
                RegionCode = "E12000005",
                RegionName = "West Midlands",
                LaName = "Birmingham",
                NewLaCode = "E08000025",
            },
            new LocalAuthority
            {
                OldLaCode = 331,
                RegionCode = "E12000005",
                RegionName = "West Midlands",
                LaName = "Coventry",
                NewLaCode = "E08000026",
            },
            new LocalAuthority
            {
                OldLaCode = 332,
                RegionCode = "E12000005",
                RegionName = "West Midlands",
                LaName = "Dudley",
                NewLaCode = "E08000027",
            },
            new LocalAuthority
            {
                OldLaCode = 884,
                RegionCode = "E12000005",
                RegionName = "West Midlands",
                LaName = "Herefordshire, County of",
                NewLaCode = "E06000019",
            },
            new LocalAuthority
            {
                OldLaCode = 333,
                RegionCode = "E12000005",
                RegionName = "West Midlands",
                LaName = "Sandwell",
                NewLaCode = "E08000028",
            },
            new LocalAuthority
            {
                OldLaCode = 893,
                RegionCode = "E12000005",
                RegionName = "West Midlands",
                LaName = "Shropshire",
                NewLaCode = "E06000051",
            },
            new LocalAuthority
            {
                OldLaCode = 334,
                RegionCode = "E12000005",
                RegionName = "West Midlands",
                LaName = "Solihull",
                NewLaCode = "E08000029",
            },
            new LocalAuthority
            {
                OldLaCode = 860,
                RegionCode = "E12000005",
                RegionName = "West Midlands",
                LaName = "Staffordshire",
                NewLaCode = "E10000028",
            },
            new LocalAuthority
            {
                OldLaCode = 861,
                RegionCode = "E12000005",
                RegionName = "West Midlands",
                LaName = "Stoke-on-Trent",
                NewLaCode = "E06000021",
            },
            new LocalAuthority
            {
                OldLaCode = 894,
                RegionCode = "E12000005",
                RegionName = "West Midlands",
                LaName = "Telford and Wrekin",
                NewLaCode = "E06000020",
            },
            new LocalAuthority
            {
                OldLaCode = 335,
                RegionCode = "E12000005",
                RegionName = "West Midlands",
                LaName = "Walsall",
                NewLaCode = "E08000030",
            },
            new LocalAuthority
            {
                OldLaCode = 937,
                RegionCode = "E12000005",
                RegionName = "West Midlands",
                LaName = "Warwickshire",
                NewLaCode = "E10000031",
            },
            new LocalAuthority
            {
                OldLaCode = 336,
                RegionCode = "E12000005",
                RegionName = "West Midlands",
                LaName = "Wolverhampton",
                NewLaCode = "E08000031",
            },
            new LocalAuthority
            {
                OldLaCode = 885,
                RegionCode = "E12000005",
                RegionName = "West Midlands",
                LaName = "Worcestershire",
                NewLaCode = "E10000034",
            },
            new LocalAuthority
            {
                OldLaCode = 822,
                RegionCode = "E12000006",
                RegionName = "East of England",
                LaName = "Bedford",
                NewLaCode = "E06000055",
            },
            new LocalAuthority
            {
                OldLaCode = 873,
                RegionCode = "E12000006",
                RegionName = "East of England",
                LaName = "Cambridgeshire",
                NewLaCode = "E10000003",
            },
            new LocalAuthority
            {
                OldLaCode = 823,
                RegionCode = "E12000006",
                RegionName = "East of England",
                LaName = "Central Bedfordshire",
                NewLaCode = "E06000056",
            },
            new LocalAuthority
            {
                OldLaCode = 881,
                RegionCode = "E12000006",
                RegionName = "East of England",
                LaName = "Essex",
                NewLaCode = "E10000012",
            },
            new LocalAuthority
            {
                OldLaCode = 919,
                RegionCode = "E12000006",
                RegionName = "East of England",
                LaName = "Hertfordshire",
                NewLaCode = "E10000015",
            },
            new LocalAuthority
            {
                OldLaCode = 821,
                RegionCode = "E12000006",
                RegionName = "East of England",
                LaName = "Luton",
                NewLaCode = "E06000032",
            },
            new LocalAuthority
            {
                OldLaCode = 926,
                RegionCode = "E12000006",
                RegionName = "East of England",
                LaName = "Norfolk",
                NewLaCode = "E10000020",
            },
            new LocalAuthority
            {
                OldLaCode = 874,
                RegionCode = "E12000006",
                RegionName = "East of England",
                LaName = "Peterborough",
                NewLaCode = "E06000031",
            },
            new LocalAuthority
            {
                OldLaCode = 882,
                RegionCode = "E12000006",
                RegionName = "East of England",
                LaName = "Southend-on-Sea",
                NewLaCode = "E06000033",
            },
            new LocalAuthority
            {
                OldLaCode = 935,
                RegionCode = "E12000006",
                RegionName = "East of England",
                LaName = "Suffolk",
                NewLaCode = "E10000029",
            },
            new LocalAuthority
            {
                OldLaCode = 883,
                RegionCode = "E12000006",
                RegionName = "East of England",
                LaName = "Thurrock",
                NewLaCode = "E06000034",
            },
            new LocalAuthority
            {
                OldLaCode = 301,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Barking and Dagenham",
                NewLaCode = "E09000002",
            },
            new LocalAuthority
            {
                OldLaCode = 302,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Barnet",
                NewLaCode = "E09000003",
            },
            new LocalAuthority
            {
                OldLaCode = 303,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Bexley",
                NewLaCode = "E09000004",
            },
            new LocalAuthority
            {
                OldLaCode = 304,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Brent",
                NewLaCode = "E09000005",
            },
            new LocalAuthority
            {
                OldLaCode = 305,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Bromley",
                NewLaCode = "E09000006",
            },
            new LocalAuthority
            {
                OldLaCode = 202,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Camden",
                NewLaCode = "E09000007",
            },
            new LocalAuthority
            {
                OldLaCode = 201,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "City of London",
                NewLaCode = "E09000001",
            },
            new LocalAuthority
            {
                OldLaCode = 306,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Croydon",
                NewLaCode = "E09000008",
            },
            new LocalAuthority
            {
                OldLaCode = 307,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Ealing",
                NewLaCode = "E09000009",
            },
            new LocalAuthority
            {
                OldLaCode = 308,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Enfield",
                NewLaCode = "E09000010",
            },
            new LocalAuthority
            {
                OldLaCode = 203,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Greenwich",
                NewLaCode = "E09000011",
            },
            new LocalAuthority
            {
                OldLaCode = 204,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Hackney",
                NewLaCode = "E09000012",
            },
            new LocalAuthority
            {
                OldLaCode = 205,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Hammersmith and Fulham",
                NewLaCode = "E09000013",
            },
            new LocalAuthority
            {
                OldLaCode = 309,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Haringey",
                NewLaCode = "E09000014",
            },
            new LocalAuthority
            {
                OldLaCode = 310,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Harrow",
                NewLaCode = "E09000015",
            },
            new LocalAuthority
            {
                OldLaCode = 311,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Havering",
                NewLaCode = "E09000016",
            },
            new LocalAuthority
            {
                OldLaCode = 312,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Hillingdon",
                NewLaCode = "E09000017",
            },
            new LocalAuthority
            {
                OldLaCode = 313,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Hounslow",
                NewLaCode = "E09000018",
            },
            new LocalAuthority
            {
                OldLaCode = 206,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Islington",
                NewLaCode = "E09000019",
            },
            new LocalAuthority
            {
                OldLaCode = 207,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Kensington and Chelsea",
                NewLaCode = "E09000020",
            },
            new LocalAuthority
            {
                OldLaCode = 314,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Kingston upon Thames",
                NewLaCode = "E09000021",
            },
            new LocalAuthority
            {
                OldLaCode = 208,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Lambeth",
                NewLaCode = "E09000022",
            },
            new LocalAuthority
            {
                OldLaCode = 209,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Lewisham",
                NewLaCode = "E09000023",
            },
            new LocalAuthority
            {
                OldLaCode = 315,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Merton",
                NewLaCode = "E09000024",
            },
            new LocalAuthority
            {
                OldLaCode = 316,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Newham",
                NewLaCode = "E09000025",
            },
            new LocalAuthority
            {
                OldLaCode = 317,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Redbridge",
                NewLaCode = "E09000026",
            },
            new LocalAuthority
            {
                OldLaCode = 318,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Richmond upon Thames",
                NewLaCode = "E09000027",
            },
            new LocalAuthority
            {
                OldLaCode = 210,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Southwark",
                NewLaCode = "E09000028",
            },
            new LocalAuthority
            {
                OldLaCode = 319,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Sutton",
                NewLaCode = "E09000029",
            },
            new LocalAuthority
            {
                OldLaCode = 211,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Tower Hamlets",
                NewLaCode = "E09000030",
            },
            new LocalAuthority
            {
                OldLaCode = 320,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Waltham Forest",
                NewLaCode = "E09000031",
            },
            new LocalAuthority
            {
                OldLaCode = 212,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Wandsworth",
                NewLaCode = "E09000032",
            },
            new LocalAuthority
            {
                OldLaCode = 213,
                RegionCode = "E12000007",
                RegionName = "London",
                LaName = "Westminster",
                NewLaCode = "E09000033",
            },
            new LocalAuthority
            {
                OldLaCode = 867,
                RegionCode = "E12000008",
                RegionName = "South East",
                LaName = "Bracknell Forest",
                NewLaCode = "E06000036",
            },
            new LocalAuthority
            {
                OldLaCode = 846,
                RegionCode = "E12000008",
                RegionName = "South East",
                LaName = "Brighton and Hove",
                NewLaCode = "E06000043",
            },
            new LocalAuthority
            {
                OldLaCode = 825,
                RegionCode = "E12000008",
                RegionName = "South East",
                LaName = "Buckinghamshire",
                NewLaCode = "E06000060",
            },
            new LocalAuthority
            {
                OldLaCode = 845,
                RegionCode = "E12000008",
                RegionName = "South East",
                LaName = "East Sussex",
                NewLaCode = "E10000011",
            },
            new LocalAuthority
            {
                OldLaCode = 850,
                RegionCode = "E12000008",
                RegionName = "South East",
                LaName = "Hampshire",
                NewLaCode = "E10000014",
            },
            new LocalAuthority
            {
                OldLaCode = 921,
                RegionCode = "E12000008",
                RegionName = "South East",
                LaName = "Isle of Wight",
                NewLaCode = "E06000046",
            },
            new LocalAuthority
            {
                OldLaCode = 886,
                RegionCode = "E12000008",
                RegionName = "South East",
                LaName = "Kent",
                NewLaCode = "E10000016",
            },
            new LocalAuthority
            {
                OldLaCode = 887,
                RegionCode = "E12000008",
                RegionName = "South East",
                LaName = "Medway",
                NewLaCode = "E06000035",
            },
            new LocalAuthority
            {
                OldLaCode = 826,
                RegionCode = "E12000008",
                RegionName = "South East",
                LaName = "Milton Keynes",
                NewLaCode = "E06000042",
            },
            new LocalAuthority
            {
                OldLaCode = 931,
                RegionCode = "E12000008",
                RegionName = "South East",
                LaName = "Oxfordshire",
                NewLaCode = "E10000025",
            },
            new LocalAuthority
            {
                OldLaCode = 851,
                RegionCode = "E12000008",
                RegionName = "South East",
                LaName = "Portsmouth",
                NewLaCode = "E06000044",
            },
            new LocalAuthority
            {
                OldLaCode = 870,
                RegionCode = "E12000008",
                RegionName = "South East",
                LaName = "Reading",
                NewLaCode = "E06000038",
            },
            new LocalAuthority
            {
                OldLaCode = 871,
                RegionCode = "E12000008",
                RegionName = "South East",
                LaName = "Slough",
                NewLaCode = "E06000039",
            },
            new LocalAuthority
            {
                OldLaCode = 852,
                RegionCode = "E12000008",
                RegionName = "South East",
                LaName = "Southampton",
                NewLaCode = "E06000045",
            },
            new LocalAuthority
            {
                OldLaCode = 936,
                RegionCode = "E12000008",
                RegionName = "South East",
                LaName = "Surrey",
                NewLaCode = "E10000030",
            },
            new LocalAuthority
            {
                OldLaCode = 869,
                RegionCode = "E12000008",
                RegionName = "South East",
                LaName = "West Berkshire",
                NewLaCode = "E06000037",
            },
            new LocalAuthority
            {
                OldLaCode = 938,
                RegionCode = "E12000008",
                RegionName = "South East",
                LaName = "West Sussex",
                NewLaCode = "E10000032",
            },
            new LocalAuthority
            {
                OldLaCode = 868,
                RegionCode = "E12000008",
                RegionName = "South East",
                LaName = "Windsor and Maidenhead",
                NewLaCode = "E06000040",
            },
            new LocalAuthority
            {
                OldLaCode = 872,
                RegionCode = "E12000008",
                RegionName = "South East",
                LaName = "Wokingham",
                NewLaCode = "E06000041",
            },
            new LocalAuthority
            {
                OldLaCode = 800,
                RegionCode = "E12000009",
                RegionName = "South West",
                LaName = "Bath and North East Somerset",
                NewLaCode = "E06000022",
            },
            new LocalAuthority
            {
                OldLaCode = 839,
                RegionCode = "E12000009",
                RegionName = "South West",
                LaName = "Bournemouth, Christchurch and Poole",
                NewLaCode = "E06000058",
            },
            new LocalAuthority
            {
                OldLaCode = 801,
                RegionCode = "E12000009",
                RegionName = "South West",
                LaName = "Bristol, City of",
                NewLaCode = "E06000023",
            },
            new LocalAuthority
            {
                OldLaCode = 908,
                RegionCode = "E12000009",
                RegionName = "South West",
                LaName = "Cornwall",
                NewLaCode = "E06000052",
            },
            new LocalAuthority
            {
                OldLaCode = 878,
                RegionCode = "E12000009",
                RegionName = "South West",
                LaName = "Devon",
                NewLaCode = "E10000008",
            },
            new LocalAuthority
            {
                OldLaCode = 838,
                RegionCode = "E12000009",
                RegionName = "South West",
                LaName = "Dorset",
                NewLaCode = "E06000059",
            },
            new LocalAuthority
            {
                OldLaCode = 916,
                RegionCode = "E12000009",
                RegionName = "South West",
                LaName = "Gloucestershire",
                NewLaCode = "E10000013",
            },
            new LocalAuthority
            {
                OldLaCode = 420,
                RegionCode = "E12000009",
                RegionName = "South West",
                LaName = "Isles of Scilly",
                NewLaCode = "E06000053",
            },
            new LocalAuthority
            {
                OldLaCode = 802,
                RegionCode = "E12000009",
                RegionName = "South West",
                LaName = "North Somerset",
                NewLaCode = "E06000024",
            },
            new LocalAuthority
            {
                OldLaCode = 879,
                RegionCode = "E12000009",
                RegionName = "South West",
                LaName = "Plymouth",
                NewLaCode = "E06000026",
            },
            new LocalAuthority
            {
                OldLaCode = 933,
                RegionCode = "E12000009",
                RegionName = "South West",
                LaName = "Somerset",
                NewLaCode = "E06000066",
            },
            new LocalAuthority
            {
                OldLaCode = 803,
                RegionCode = "E12000009",
                RegionName = "South West",
                LaName = "South Gloucestershire",
                NewLaCode = "E06000025",
            },
            new LocalAuthority
            {
                OldLaCode = 866,
                RegionCode = "E12000009",
                RegionName = "South West",
                LaName = "Swindon",
                NewLaCode = "E06000030",
            },
            new LocalAuthority
            {
                OldLaCode = 880,
                RegionCode = "E12000009",
                RegionName = "South West",
                LaName = "Torbay",
                NewLaCode = "E06000027",
            },
            new LocalAuthority
            {
                OldLaCode = 865,
                RegionCode = "E12000009",
                RegionName = "South West",
                LaName = "Wiltshire",
                NewLaCode = "E06000054",
            }
        );
    }
}
