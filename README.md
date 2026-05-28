# Muon Sync

Muon Sync is a .NET Core console application for processing, cleaning, synchronizing, and analyzing muon‑detector CSV data together with positional GFX files.  
All CSV operations are performed row‑by‑row to avoid high memory usage.



# Features

## 1. CSV Cleaning (`clean`)
Input: input.csv  
Output: input_cleaned.csv

Removes rows where waveform data contains mostly zeros and drops the unused last column.  
The cleaned file contains only valid detector events.

## 2. Waveform Integration (`integral`)
Input: input_cleaned.csv  
Output: input_integrated.csv

Replaces the 128‑sample waveform with a single integrated value (UInt64).  
This reduces data size and prepares the file for coincidence analysis.

## 3. Coincidence Detection (`coincidence`)
Input: input_integrated.csv  
Output: coincidence.csv

A 20‑sample rolling window is used to detect events occurring within 1 microsecond across channels.  
Each output row contains:
- Event time of the first event in the coincidence
- Average integrated value of all events in the coincidence
- Eight channel flags (0 or 1) indicating which channels participated

## 4. Event Count per Minute (`count`)
Input: coincidence.csv  
Output: event_counts_per_minute.csv

Aggregates coincidences into 1‑minute intervals based on nanosecond timestamps.  
Each output row contains:
- Minute index (starting from 0)
- Number of events in that minute
- Average integrated value for that minute  
Minutes with zero events are also written.

## 5. GFX Combination (`combine [shift]`)
Input 1: event_counts_per_minute.csv  
Input 2: input.gfx  
Output: tunnel.gfx

Synchronizes event counts with GPS data from a GFX file.  
The Z‑coordinate in the GFX file is replaced with (event_count × 30) for visualization.  
A shift parameter (in seconds) allows adjusting for devices that did not start at the same time.

The resulting tunnel.gfx can be loaded into mapping software to visualize muon intensity along a route, such as detecting tunnels where muon counts decrease.

# CSV Format

The CSV file contains events from a mobile muon detector with 8 channels (0–7).  
Each row contains:
- Channel index  
- Event timestamp in nanoseconds  
- 128 waveform samples

# Example data
GPS track viewer tool:
https://www.therideatlas.com/tools/gpx-viewer/

The GPX file where the height data is replaced with muon count:

input_data/tunnel.gpx

Original data files:

input_data/input.csv // moun waveforms with event time

input_data/input.gpx // gps tracklog



# Program usage
MuonSync - Muon detector data synchronization tool

Usage: MuonSync <command> [options]

Commands:

clean - Remove zero-waveform rows from input CSV

integral - Replace 128-sample waveforms with integrated values

coincidence - Find multi-channel coincidences within 1 microsecond

count - Count coincidences per minute

combine [shift_sec] - Combine event counts with GPS track (shift in seconds)

