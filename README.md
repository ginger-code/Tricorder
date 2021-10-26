# Tricorder

A utility for scanning and querying HL7 messages in bulk.

Will process all feeds with a `.hl7` extension

## Usage

Scan a directory to accumulate tallies of values for the PV1.3.1 component:

`Tricorder.exe scan PV1.3.1 "C:\Data\Feeds\"`

Scan a directory to accumulate tallies of values for the PT1.1 field:

`Tricorder.exe scan GT1.1 "C:\Data\Feeds\"`

Scan a directory to accumulate tallies of values for the entire PID segment:

`Tricorder.exe scan PID "C:\Data\Feeds\"`
