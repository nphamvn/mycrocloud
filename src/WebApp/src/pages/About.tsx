import { CodeSnippet } from "../components/code-snippet";

export default function AppInfo() {
  const message = JSON.stringify({ ...import.meta.env, __COMMIT_HASH__ }, null, 4);
  return (
    <div>
      <h1>App Info</h1>
      <CodeSnippet title="env" code={message} />
    </div>
  );
}
