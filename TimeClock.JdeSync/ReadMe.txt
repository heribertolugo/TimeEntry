------------------------------------
JdeSync
------------------------------------
This application can import JDE data into the TimeClock database, and import into JDE time entry data from the TimeClock database.

To configure the database connections, please see appsettings.json

------------------------------------
JobTypeSteps not inserted/updated for users with multiple choice:
this is only meant to be done on first run.
to force the application to insert this data, delete the file in the application's execution directory called "NotFirstRun"

------------------------------------
To Run:
run application using one or more of the following command line arguments:
-sync
-timeentry

The -sync argument updates the TimeClock database with JDE data.
The -timeentry argument imports time entry records into JDE.
Command line arguments should be seperated with a space like so:
>start JdeSync.exe -sync -timeentry
------------------------------------

/**********************************\
Configurations in appsettings.json:
\**********************************/
------------------------------------
Logging:
Verbosity can be configured in the logging section.
The supported logging providers are already set in the appsettings.json, which are Console and File.
To set verbosity please set using restrictedToMinimumLevel inside Serilog section.
The possible verbosity levels are as follows:
None (no logging through the provider)
Debug (data for debugging - may not be used)
Trace (extreme verbosity)
Information (data output within each step of application logic process)
Warning (scenerios which are of concern or important output, such as app start and app end)
Error (errors which occured during application process)
Critical (scenarios which may interfere with system, such as low disk or ram - not currently used)

The log file has a limit set on how many log files can exist and that each files max size is.
these are set through retainedFileCountLimit and fileSizeLimitBytes respectively.
Whether the file should roll when max file size is reached can be toggled using rollOnFileSizeLimit.
The path to the log file uses the application's directory as root and can be configured with path.
The outputTemplate is purposely disabled to allow standard logging which can be used with log reader applications.
It can be enabled and configured as desired or needed.
An interval for when a log file should role regardless of size can be set using rollingInterval.
Values for rollingInterval are limited to:
Infinite (The log file will never roll; no time period information will be appended to the log file name.)
Year (Roll every year. Filenames will have a four-digit year appended in the pattern yyyy.)
Month (Roll every calendar month. Filenames will have yyyyMM appended.)
Day (Roll every day. Filenames will have yyyyMMdd appended.)
Hour (Roll every hour. Filenames will have yyyyMMddHH appended.)
Minute (Roll every minute. Filenames will have yyyyMMddHHmm appended.)
------------------------------------

------------------------------------
Database Connections:
The database connection for JDE or TimeClock can be configured using ConnectionStrings section.
The names are self explanatory and have a default value showing the expected format of the connection parameters.
timeclocksqlSA is a high level user account which is not currently used, and so it can be omitted.
------------------------------------

------------------------------------
Pay Configuration (OverTime, Straight Pay):
sample data:

{ 
  "defaultOvertimeRule": {
    "afterHours": 40.0,
    "overtimeThreshold": "Week",
    "specialPay": {
      "holliday": {
        "amount": 1.5,
        "increaseRateType": "Percentage"
      },
      "saturday": {
        "amount": 2.0,
        "increaseRateType": "FlatIncrease"
      },
      "sunday": {
        "amount": 50.0,
        "increaseRateType": "FlatPay"
      }
    }
  },
  "specialOvertimeRules": [
    {
      "objects": "JN;UUU",
      "afterHours": 8.0,
      "overtimeThreshold": "Day",
      "specialPay": {
        "holliday": {
          "amount": 2.5,
          "increaseRateType": "Percentage"
        },
        "saturday": {
          "amount": 10.0,
          "increaseRateType": "FlatIncrease"
        },
        "sunday": {
          "amount": 2.0,
          "increaseRateType": "Percentage"
        }
      },
      "objectsType": "Unions"
    },
    {
      "objects": "UOA",
      "afterHours": 6.0,
      "overtimeThreshold": "Day",
      "specialPay": {
        "holliday": {
          "amount": 3.0,
          "increaseRateType": "Percentage"
        },
        "saturday": null,
        "sunday": null
      },
      "objectsType": "Unions"
    },
    {
      "objects": "6001;6005;2014",
      "afterHours": 6.0,
      "overtimeThreshold": "Week",
      "specialPay": {
        "holliday": {
          "amount": 15.0,
          "increaseRateType": "FlatIncrease"
        },
        "saturday": {
          "amount": 15.0,
          "increaseRateType": "FlatIncrease"
        },
        "sunday": {
          "amount": 15.0,
          "increaseRateType": "FlatIncrease"
        }
      },
      "objectsType": "Projects"
    }
  ],
  "jobTypePayRules": [
    {
      "jobTypes": "54;88;76;98",
      "increaseRate": {
        "amount": 5.0,
        "increaseRateType": "FlatIncrease"
      },
      "specialPay": {
        "holliday": {
          "amount": 1.5,
          "increaseRateType": "Percentage"
        },
        "saturday": {
          "amount": 8.0,
          "increaseRateType": "FlatIncrease"
        },
        "sunday": {
          "amount": 10.0,
          "increaseRateType": "FlatIncrease"
        }
      }
    },
    {
      "jobTypes": "22;54;12;33;45;78;99",
      "increaseRate": {
        "amount": 2.0,
        "increaseRateType": "FlatIncrease"
      },
      "specialPay": {
        "holliday": {
          "amount": 1.25,
          "increaseRateType": "Percentage"
        },
        "saturday": {
          "amount": 3.0,
          "increaseRateType": "FlatIncrease"
        },
        "sunday": {
          "amount": 5.0,
          "increaseRateType": "FlatIncrease"
        }
      }
    }
  ]
}

------------------------------------

All other items in appsettings.json should not be changed, except by developers in rare circumstances.