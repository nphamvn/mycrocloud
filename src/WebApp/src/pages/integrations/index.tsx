import { useAuth0 } from "@auth0/auth0-react";
import { useContext, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { AppContext } from "../apps";

export interface GitHubRepo {
  id: number;
  name: string;
  fullName: string;
  description: string;
  createdAt: string;
  updatedAt: string;
}

export default function Integrations() {
  const { getAccessTokenSilently } = useAuth0();
  const app = useContext(AppContext)!;
  console.log(app);
  const navigate = useNavigate();

  const [repoFullName, setRepoFullName] = useState<string | null>(null);
  const [githubAuthorized, setGitHubAuthorized] = useState(false);
  const [githubRepos, setGitHubRepos] = useState<GitHubRepo[]>([]);

  useEffect(() => {
    (async () => {
      const accessToken = await getAccessTokenSilently();
      const res = await fetch(`/api/me`, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
      if (res.ok) {
        const { connections } = await res.json();
        setGitHubAuthorized(
          connections.find((c: any) => c.provider === "GitHub"),
        );
      }
    })();
  }, []);

  useEffect(() => {
    if (!githubAuthorized) return;

    (async () => {
      const accessToken = await getAccessTokenSilently();
      const res = await fetch(`/api/integrations/github/repos`, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
      if (res.ok) {
        const repos = (await res.json()) as GitHubRepo[];
        setGitHubRepos(repos);
      }
    })();
  }, [githubAuthorized]);

  const onConnectClick = async () => {
    const accessToken = await getAccessTokenSilently();
    const connect = !app.gitHubRepoFullName;
    const res = await fetch(
      `/api/integrations/app-github?appId=${app.id}&repoFullName=${repoFullName}&connect=${connect}`,
      {
        method: "POST",
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      },
    );
    if (res.ok) {
      //todo: update app state
    }
  };

  return (
    <div className="p-2">
      <h1 className="font-bold">Integrations</h1>
      <div className="mt-4 rounded-sm border p-2">
        {githubAuthorized === false ? (
          <button
            onClick={() => {
              navigate("/settings");
            }}
            className="rounded-sm border bg-gray-900 px-2 py-1.5 text-white"
          >
            Go to Settings to connect GitHub
          </button>
        ) : (
          <div className="flex">
            <select
              value={repoFullName || ""}
              onChange={(e) => {
                setRepoFullName(e.target.value);
              }}
              className="border px-2 py-1.5"
            >
              <option>Select a repository</option>
              {githubRepos.map((repo) => (
                <option key={repo.fullName} value={repo.fullName}>
                  {repo.name}
                </option>
              ))}
            </select>
            <button
              onClick={onConnectClick}
              className="ms-2 bg-primary px-2 text-white"
            >
              {app.gitHubRepoFullName ? "Disconnect" : "Connect"}
            </button>
          </div>
        )}
        <p className="mt-2 border p-2 text-sm text-slate-500">
          You will be redirected to GitHub to authorize the integration. <br />
          After authorization, we will store the access token securely and use
          it to deploy your repositories. <br />
          First, we will add a webhook to your repository to listen for changes.
          <br />
          Then, we will clone your repository to our servers and build your
          project. <br />
          The build output files will be uploaded to your app's files storage.
          <br />
          Finally, we will generate route for your app to serve the files.
        </p>
      </div>
    </div>
  );
}
