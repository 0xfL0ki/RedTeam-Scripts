import os
import random
import base64
from Crypto.Cipher import AES

def generate_random_key(key_size):
    return os.urandom(key_size)

def write_key_to_file(key, filename):
    with open(filename, 'wb') as f:
        f.write(key)

def main():
    key_size = 32 # AES key size is 32 bytes (256 bits)
    key = generate_random_key(key_size)
    encoded_key = base64.b64encode(key)
    filename = "aes_key.txt"
    write_key_to_file(encoded_key, filename)
    print(f"AES key is generated and written to {filename}")

if __name__ == "__main__":
    main()
