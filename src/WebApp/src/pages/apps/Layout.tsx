import { Link, Outlet, useMatch, useParams } from "react-router-dom";
import { AppContext } from ".";
import { useEffect, useState } from "react";
import IApp from "./App";
import { Breadcrumb } from "flowbite-react";
import { useAuth0 } from "@auth0/auth0-react";

export default function AppLayout() {
  const { getAccessTokenSilently } = useAuth0();
  const appId = parseInt(useParams()["appId"]!.toString());
  const [app, setApp] = useState<IApp>();

  const isMatchOverview = useMatch("/apps/:appId");
  const isMatchRoutes = useMatch("/apps/:appId/routes");
  const isMatchLogs = useMatch("/apps/:appId/logs");
  const isMatchAuthenticationSchemes = useMatch(
    "/apps/:appId/authentications/schemes",
  );
  const isMatchAuthenticationSettings = useMatch(
    "/apps/:appId/authentications/settings",
  );
  const isMatchTextStorages = useMatch("/apps/:appId/storages/textstorages");
  const isMatchVariables = useMatch("/apps/:appId/storages/variables");

  useEffect(() => {
    const getApp = async () => {
      const accessToken = await getAccessTokenSilently();
      const app = (await (
        await fetch(`/api/apps/${appId}`, {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        })
      ).json()) as IApp;
      setApp(app);
      setDocumentTitle();
    };
    getApp();
  }, []);

  function setDocumentTitle() {
    if (!app) {
      return;
    }
    let title;
    if (isMatchOverview) {
      title = app.name + " - Overview";
    } else if (isMatchRoutes) {
      title = app.name + " - Routes";
    } else if (isMatchLogs) {
      title = app.name + " - Logs";
    } else {
      title = app.name;
    }
    document.title = title;
  }
  if (!app) {
    return <h1>Loading...</h1>;
  }
  return (
    <AppContext.Provider value={app}>
      <div className="">
        <Breadcrumb className="p-1">
          <Breadcrumb.Item>
            <Link to="/">Home</Link>
          </Breadcrumb.Item>
          <Breadcrumb.Item>
            <Link to="/apps">Apps</Link>
          </Breadcrumb.Item>
          <Breadcrumb.Item>{app.name}</Breadcrumb.Item>
        </Breadcrumb>
        <div className="flex min-h-screen border">
          <div className="flex w-28 flex-col space-y-0.5 border-r p-1">
            <Link
              to=""
              className={`text-xs ${isMatchOverview ? "text-primary" : ""}`}
            >
              Overview
            </Link>
            <Link
              to="routes"
              className={`text-xs ${isMatchRoutes ? "text-primary" : ""}`}
            >
              Routes
            </Link>
            <div className="text-xs">
              Authentications
              <div className="flex flex-col pl-1">
                <Link
                  to="authentications/schemes"
                  className={`text-xs ${
                    isMatchAuthenticationSchemes ? "text-primary" : ""
                  }`}
                >
                  Schemes
                </Link>
                <Link
                  to="authentications/settings"
                  className={`text-xs ${
                    isMatchAuthenticationSettings ? "text-primary" : ""
                  }`}
                >
                  Settings
                </Link>
              </div>
            </div>
            <div className="text-xs">
              Storages
              <div className="flex flex-col px-1">
                <Link
                  to="storages/textstorages"
                  className={`text-xs ${
                    isMatchTextStorages ? "text-primary" : ""
                  }`}
                >
                  Text Storages
                </Link>
                <Link
                  to="storages/variables"
                  className={`text-xs ${
                    isMatchVariables ? "text-primary" : ""
                  }`}
                >
                  Variables
                </Link>
              </div>
            </div>
            <Link
              to="logs"
              className={`text-xs ${isMatchLogs ? "text-primary" : ""}`}
            >
              Logs
            </Link>
          </div>
          <div className="flex-1">
            <Outlet />
          </div>
        </div>
      </div>
    </AppContext.Provider>
  );
}
