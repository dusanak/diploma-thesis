import csv

json = "[\n"
header = None

with open('Data.csv', newline='') as csvfile:
    datareader = csv.reader(csvfile, delimiter=',')
    for row in datareader:
        if header is None:
            header = row
        else:
            json += "{"
            for name, val in zip(header, row):
                json += "\""
                json += name
                json += "\""
                json += ":"
                json += "\""
                json += val
                json += "\""
                json += ","
            json = json[:-1]
            json += "},\n"
    json = json[:-2] + "\n"
    json += "]"

with open("Data.json", "w") as outputfile:
    outputfile.write(json)