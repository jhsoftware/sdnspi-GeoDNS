# GeoDNS plug-in data file format

- The file name should use the extension ".geodns"

- The first 6 bytes of the file must be the ACSII characters "GEODNS" (all upper case).

- The rest of the file is compressed using "DeflateStream". The compressed data stream consist of the following:

- Number of IPv4 entries as 32 bit integer (4 bytes).

- For each IPv4 entry:

    - From-IP-address (4 bytes)
    - To-IP-address (4 bytes)
    - ISO country code - 2 ASCII letters (2 bytes)

- Number of IPv6 entries as 32 bit integer (4 bytes).

- For each IPv6 entry:

    - From-IP-address (16 bytes)
    - To-IP-address (16 bytes)
    - ISO country code - 2 ASCII letters (2 bytes)

The IP entries must be sorted (by From-IP-address) and may not overlap.
Any immediately neighbouring ranges with same country-code should be merged.
