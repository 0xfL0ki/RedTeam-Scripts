import sys

data = open(sys.argv[1]).read()

words = [ "list" ]

output = ""
iterator = 0
while iterator < len(data):
        output += '"%s",' % words[int(ord(data[iterator]))]
        iterator += 1

print (output)
with open("encoded_shellcode.txt", "w") as output_file:
    output_file.write(output)