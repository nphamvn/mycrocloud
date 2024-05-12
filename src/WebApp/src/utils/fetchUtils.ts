export function ensureSuccess(response: Response, message?: string): Response {
  if (response.ok) {
    return response;
  } else {
    if (message) {
      throw new Error(message);
    } else {
      throw new Error(`Failed to fetch: ${response.statusText}`);
    }
  }
}
