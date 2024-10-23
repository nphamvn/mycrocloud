import { useAuth0 } from "@auth0/auth0-react";
import { useEffect, useState } from "react";
import { useLocation, useParams } from "react-router-dom";

const CLIENT_ID = "Ov23liBsXIEMEdjwuUQM";
const REDIRECT_URI = "http://localhost:5173/integrations/callback/github";

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
  const appId = parseInt(useParams()["appId"]!.toString());
  const { pathname } = useLocation();

  const [repoId, setRepoId] = useState<number | null>(null);
  const [githubAuthorized, setGitHubAuthorized] = useState(true);
  const [githubRepos, setGitHubRepos] = useState<GitHubRepo[]>([]);

  const onClickGitHubIntegration = async () => {
    let authUrl = `https://github.com/login/oauth/authorize?client_id=${CLIENT_ID}&redirect_uri=${REDIRECT_URI}&scope=repo`;
    const state = encodeURIComponent(JSON.stringify({ pathname }));
    authUrl += `&state=${state}`;
    window.location.href = authUrl;
  };

  const onConnectClick = async () => {
    const accessToken = await getAccessTokenSilently();
    const res = await fetch(
      `/api/integrations/app-github?appId=${appId}&repoId=${repoId}`,
      {
        method: "POST",
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      },
    );
  };

  useEffect(() => {
    (async () => {
      const accessToken = await getAccessTokenSilently();
      const res = await fetch(`/api/integrations/github/repos`, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
      if (res.ok) {
        setGitHubAuthorized(true);
        const repos = (await res.json()) as GitHubRepo[];
        setGitHubRepos(repos);
      } else if (res.status === 401) {
        setGitHubAuthorized(false);
      }
    })();
  }, []);

  return (
    <div className="p-2">
      <h1 className="font-bold">Integrations</h1>
      <div className="mt-4 rounded-sm border p-2">
        {githubAuthorized === false ? (
          <button
            onClick={onClickGitHubIntegration}
            className="rounded-sm border bg-gray-900 px-2 py-1.5 text-white"
          >
            Connect to GitHub
          </button>
        ) : (
          <div className="flex">
            <select
              value={repoId || ""}
              onChange={(e) => {
                setRepoId(parseInt(e.target.value));
              }}
              className="border px-2 py-1.5"
            >
              <option>Select a repository</option>
              {githubRepos.map((repo) => (
                <option key={repo.id} value={repo.fullName}>
                  {repo.fullName}
                </option>
              ))}
            </select>
            <button
              onClick={onConnectClick}
              className="ms-2 bg-primary px-2 text-white"
            >
              Connect
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
