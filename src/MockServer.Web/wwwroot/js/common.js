const commonHttpMethods = [
  "GET",
  "HEAD",
  "POST",
  "PUT",
  "DELETE",
  "CONNECT",
  "OPTIONS",
  "TRACE",
  "PATCH",
];

const commonStatusCodes = [
  { status: 100, name: "Continue", type: "success" },
  { status: 101, name: "Switching Protocols", type: "success" },
  { status: 200, name: "OK", type: "success" },
  { status: 201, name: "Created", type: "success" },
  { status: 202, name: "Accepted", type: "success" },
  { status: 203, name: "Non-Authoritative Information", type: "success" },
  { status: 204, name: "No Content", type: "success" },
  { status: 205, name: "Reset Content", type: "success" },
  { status: 206, name: "Partial Content", type: "success" },
  { status: 300, name: "Multiple Choices", type: "success" },
  { status: 301, name: "Moved Permanently", type: "success" },
  { status: 302, name: "Found", type: "success" },
  { status: 303, name: "See Other", type: "success" },
  { status: 304, name: "Not Modified", type: "success" },
  { status: 305, name: "Use Proxy", type: "success" },
  { status: 307, name: "Temporary Redirect", type: "success" },
  { status: 400, name: "Bad Request", type: "error" },
  { status: 401, name: "Unauthorized", type: "error" },
  { status: 402, name: "Payment Required", type: "error" },
  { status: 403, name: "Forbidden", type: "error" },
  { status: 404, name: "Not Found", type: "error" },
  { status: 405, name: "Method Not Allowed", type: "error" },
  { status: 406, name: "Not Acceptable", type: "error" },
  { status: 407, name: "Proxy Authentication Required", type: "error" },
  { status: 408, name: "Request Timeout", type: "error" },
  { status: 409, name: "Conflict", type: "error" },
  { status: 410, name: "Gone", type: "error" },
  { status: 411, name: "Length Required", type: "error" },
  { status: 412, name: "Precondition Failed", type: "error" },
  { status: 413, name: "Request Entity Too Large", type: "error" },
  { status: 414, name: "Request-URI Too Long", type: "error" },
  { status: 415, name: "Unsupported Media Type", type: "error" },
  { status: 416, name: "Requested Range Not Satisfiable", type: "error" },
  { status: 417, name: "Expectation Failed", type: "error" },
  { status: 500, name: "Internal Server Error", type: "error" },
  { status: 501, name: "Not Implemented", type: "error" },
  { status: 502, name: "Bad Gateway", type: "error" },
  { status: 503, name: "Service Unavailable", type: "error" },
  { status: 504, name: "Gateway Timeout", type: "error" },
];

const commonHeaders = [
  "Accept",
  "Accept-Charset",
  "Accept-Encoding",
  "Accept-Language",
  "Authorization",
  "Cache-Control",
  "Connection",
  "Content-Type",
  "Cookie",
  "DNT (Do Not Track)",
  "Host",
  "Keep-Alive",
  "User-Agent",
];

const commonHeadersWithValue = [
  {
    name: "Accept",
    value:
      "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
  },
  { name: "Accept-Charset", value: "utf-8" },
  { name: "Accept-Encoding", value: "gzip, deflate, br" },
  { name: "Accept-Language", value: "en-US,en;q=0.8" },
  { name: "Authorization", value: "Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==" },
  { name: "Cache-Control", value: "no-cache" },
  { name: "Connection", value: "keep-alive" },
  { name: "Content-Type", value: "application/json" },
  {
    name: "Cookie",
    value: "_ga=GA1.2.132672898.1523671234; _gid=GA1.2.564521159.1523671234",
  },
  { name: "DNT (Do Not Track)", value: "1" },
  { name: "Host", value: "www.example.com:80" },
  { name: "If-Modified-Since", value: "Mon, 27 Mar 2017 09:35:00 GMT" },
  { name: "If-None-Match", value: '"e0023aa4f"' },
  { name: "If-Range", value: '"e0023aa4f"' },
  { name: "If-Unmodified-Since", value: "Mon, 27 Mar 2017 09:35:00 GMT" },
  { name: "Keep-Alive", value: "timeout=5, max=1000" },
  { name: "Pragma", value: "no-cache" },
  { name: "Referrer", value: "https://www.google.com/" },
  { name: "TE (Transcoding Extension)", value: "trailers" },
  { name: "Upgrade", value: "HTTP/2.0, SHTTP/1.3, IRC/6.9, RTA/x11" },
  {
    name: "User-Agent",
    value:
      "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36",
  },
  { name: "Via", value: "1.0 fred, 1.1 example.com (Apache/1.1)" },
];

function addMinutes(date, minutes) {
  return new Date(date.getTime() + minutes * 60000);
}

Array.prototype.clear = function () {
  while (this.length > 0) {
    this.pop();
  }
};

function createKey(charCount) {
  var key = "";
  var possible =
    "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

  for (var i = 0; i < charCount; i++) {
    key += possible.charAt(Math.floor(Math.random() * possible.length));
  }

  return key;
}

function log(obj) {
  console.log(obj);
}
function log(name, obj) {
  var str = JSON.stringify(obj, null, 2);
  console.log(name + ": " + str);
}
