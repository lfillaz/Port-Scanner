import socket
import requests
import json
import datetime

def get_country_info(laz_ip_address):
    try:
        response = requests.get(f"http://ip-api.com/json/{laz_ip_address}")
        data = response.json()
        if data["status"] == "success":
            return f"Country: {data['country']}, Region: {data['regionName']}, City: {data['city']}"
        else:
            return "Failed to get VPS information."
    except Exception as e:
        return str(e)

def send_to_webhook(laz_webhook_url, laz_message_content):
    headers = {
        "Content-Type": "application/json",
    }
    data = {
        "content": laz_message_content,
    }
    requests.post(laz_webhook_url, data=json.dumps(data), headers=headers)

def get_service_name(laz_port):
    try:
        service_name = socket.getservbyport(laz_port)
        return f"Port {laz_port} ({service_name})"
    except (socket.error, socket.herror, socket.gaierror, socket.timeout, OSError):
        return f"Port {laz_port} (Unknown)"

def scan_all_ports(laz_ip_address, laz_webhook_url):
    country_info = get_country_info(laz_ip_address)
    
    for laz_port in range(1, 65536):
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.settimeout(1)
        result = sock.connect_ex((laz_ip_address, laz_port))
        service_name = get_service_name(laz_port)
        link = f"http://{laz_ip_address}:{laz_port}"

        if result == 0:
            message_content = f"{country_info}\n\nPort: {laz_port} is open. Service: {service_name}, Link: {link}"
            send_to_webhook(laz_webhook_url, message_content)
        else:
            print(f"Port {laz_port} is not open.")
        sock.close()

if __name__ == "__main__":
    print("""
    01010110 01010000 01010011  01010000 01101111 01110010 01110100  01010011 01100011 01100001 01101110 01101110 01100101 01110010 
    ██╗   ██╗██████╗ ███████╗    ██████╗  ██████╗ ██████╗ ████████╗    ███████╗ ██████╗ █████╗ ███╗   ██╗███╗   ██╗███████╗██████╗ 
    ██║   ██║██╔══██╗██╔════╝    ██╔══██╗██╔═══██╗██╔══██╗╚══██╔══╝    ██╔════╝██╔════╝██╔══██╗████╗  ██║████╗  ██║██╔════╝██╔══██╗
    ██║   ██║██████╔╝███████╗    ██████╔╝██║   ██║██████╔╝   ██║       ███████╗██║     ███████║██╔██╗ ██║██╔██╗ ██║█████╗  ██████╔╝
    ╚██╗ ██╔╝██╔═══╝ ╚════██║    ██╔═══╝ ██║   ██║██╔══██╗   ██║       ╚════██║██║     ██╔══██║██║╚██╗██║██║╚██╗██║██╔══╝  ██╔══██╗
     ╚████╔╝ ██║     ███████║    ██║     ╚██████╔╝██║  ██║   ██║       ███████║╚██████╗██║  ██║██║ ╚████║██║ ╚████║███████╗██║  ██║
      ╚═══╝  ╚═╝     ╚══════╝    ╚═╝      ╚═════╝ ╚═╝  ╚═╝   ╚═╝       ╚══════╝ ╚═════╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═══╝╚══════╝╚═╝  ╚═╝
      Nice tool for scan vps port by @lfillaz github                                                                                                                             
    """)
    laz_ip_address = input("Please enter the VPS Host (example: 103.101.203.43): ")
    laz_webhook_url = input("Please enter your Discord Webhook URL: ")

    scan_all_ports(laz_ip_address, laz_webhook_url)
