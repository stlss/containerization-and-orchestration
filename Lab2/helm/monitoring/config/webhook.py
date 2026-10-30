import json
import threading
from collections import deque
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer

notifications = deque(maxlen=100)
lock = threading.Lock()


class Handler(BaseHTTPRequestHandler):
    def reply(self, status, body):
        encoded = json.dumps(body, ensure_ascii=False).encode()
        self.send_response(status)
        self.send_header("Content-Type", "application/json; charset=utf-8")
        self.send_header("Content-Length", str(len(encoded)))
        self.end_headers()
        self.wfile.write(encoded)

    def do_GET(self):
        if self.path == "/health":
            self.reply(200, {"status": "ok"})
        elif self.path == "/alerts":
            with lock:
                snapshot = list(notifications)
            self.reply(200, snapshot)
        else:
            self.reply(404, {"error": "not found"})

    def do_POST(self):
        if self.path != "/alerts":
            self.reply(404, {"error": "not found"})
            return
        try:
            length = int(self.headers.get("Content-Length", "0"))
            if not 0 < length <= 1024 * 1024:
                self.reply(413, {"error": "invalid payload size"})
                return
            body = json.loads(self.rfile.read(length))
        except (ValueError, UnicodeDecodeError):
            self.reply(400, {"error": "invalid JSON"})
            return
        with lock:
            notifications.append(body)
        print(json.dumps({"event": "alert_notification", "payload": body}, ensure_ascii=False), flush=True)
        self.reply(200, {"received": True})


ThreadingHTTPServer(("0.0.0.0", 8080), Handler).serve_forever()
