import sys
import os

# check if a file name was provided as an argument
if len(sys.argv) != 2:
    print("Usage: python gen_shellcode.py <binary_file>")
    sys.exit(1)

# get the file name from the command line argument
binary_file_name = sys.argv[1]

# open the binary file in read mode
with open(binary_file_name, "rb") as binary_file:
    # read the contents of the file into a bytes object
    binary_data = binary_file.read()

# initialize an empty list to store the hex values
hex_values = []
    
# iterate over each byte in the binary data
for byte in binary_data:
        # convert the byte to its hexadecimal representation
        hex_value = hex(byte)
        
        # append the hex value to the list of hex values
        hex_values.append(hex_value)
    
# join the list of hex values into a single string, separated by commas
hex_string = ",".join(hex_values)

output = "{{bytes[{0}] = ({1})}}".format(len(binary_data), hex_string)

# print the output
# print(output)
# open the output file in write mode
print("Shellcode saved as {0}\shellcode.txt".format(os.getcwd()))
with open("shellcode.txt", "w") as output_file:
    output_file.write(output)
    
# print the hex string
# print(hex_string)