# Insert CSV hero data into MySql
# Requires pymysql: `pip install pymysql`

import sys
import csv
import pymysql.cursors

# USAGE: ./insert_heroes.py csv_file mysql_host mysql_user mysql_password mysql_db num_rows
# USAGE: ./insert_heroes.py marvel-wikia-data.csv mysql_host mysql_user mysql_password mysql_db 20
def main(argv=None):
    csv_file = argv[1]
    mysql_host = argv[2]
    mysql_user = argv[3]
    mysql_pass = argv[4]
    mysql_db = argv[5]
    max_rows = int(argv[6])

    # Connect to the database
    connection = pymysql.connect(host=mysql_host,
                             user=mysql_user,
                             password=mysql_pass,
                             db=mysql_db,
                             charset='utf8',
                             cursorclass=pymysql.cursors.DictCursor)

    with connection.cursor() as cursor:

        # read the csv and insert each row
        with open(csv_file, 'rb') as f:
            reader = csv.reader(f)
            idx = 0
            for row in reader:
                if idx == 0:
                    idx += 1
                    continue
                if idx >= max_rows:
                    break
                sql = "INSERT INTO `heroes` (`name`, `urlslug`, `identity`, `alignment`, `eye_color`, `hair_color`, `sex`, `gsm`, `alive`, `appearances`, `first_appearance`, `year`, `lon`, `lat`) VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)"

                # parse appearances as int
                appearances_str = row[10]
                appearances = 0
                if(len(appearances_str) > 0):
                    appearances = int(appearances_str)
                else:
                    continue

                # parse year as string
                year_str = row[12]
                year = 0
                if(len(year_str) > 0):
                    year = int(year_str)
                else:
                    continue
                cursor.execute(sql, (row[1], row[2], row[3], row[4], row[5], row[6], row[7], row[8], row[9], appearances, row[11], year, float(row[13]), float(row[14])))
                sys.stdout.write(".")
                sys.stdout.flush()
                connection.commit()
                idx += 1
    return 0

if __name__ == '__main__':
    sys.exit(main(sys.argv))
