import { Link, Outlet, useLocation, useParams } from "react-router-dom";
import { AppContext } from "./AppContext";
import { useEffect, useState } from "react";
import App from "./App";
import { Breadcrumb } from "flowbite-react";
import { useAuth0 } from "@auth0/auth0-react";

export default function AppIndex() {
  const { getAccessTokenSilently } = useAuth0();
  const appId = parseInt(useParams()["appId"]!.toString());
  const [app, setApp] = useState<App>();
  const { pathname } = useLocation();
  const path3 = pathname.split("/")[3];
  const path4 = pathname.split("/")[4];
  useEffect(() => {
    const getApp = async () => {
      const accessToken = await getAccessTokenSilently();
      const app = (await (
        await fetch(`/api/apps/${appId}`, {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        })
      ).json()) as App;
      setApp(app);
    };
    getApp();
  }, []);
  useEffect(() => {
    const path3 = pathname.split("/")[3];
    if (app) {
      switch (path3) {
        case "overview":
          document.title = app.name + " - Overview";
          break;
        case "routes":
          document.title = app.name + " - Routes";
          break;
        case "logs":
          document.title = app.name + " - Logs";
          break;
        default:
          document.title = app.name;
          break;
      }
    }
  }, [app, pathname]);
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
              to="overview"
              className={`text-xs ${
                path3 === "overview" ? "text-primary" : ""
              }`}
            >
              Overview
            </Link>
            <Link
              to="routes"
              className={`text-xs ${path3 === "routes" ? "text-primary" : ""}`}
            >
              Routes
            </Link>
            <div className="flex flex-col text-xs">
              Authentications
              <Link
                to="authentications/schemes"
                className={`ms-2 text-xs ${
                  path3 === "authentications" && path4 === "schemes"
                    ? "text-primary"
                    : ""
                }`}
              >
                Schemes
              </Link>
              <Link
                to="authentications/settings"
                className={`ms-2 text-xs ${
                  path3 === "authentications" && path4 === "settings"
                    ? "text-primary"
                    : ""
                }`}
              >
                Settings
              </Link>
            </div>
            <Link
              to="logs"
              className={`text-xs ${path3 === "logs" ? "text-primary" : ""}`}
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
