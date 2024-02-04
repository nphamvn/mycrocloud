import { Link, Outlet, useLocation, useParams } from "react-router-dom";
import { AppContext } from "./AppContext";
import { useEffect, useState } from "react";
import App from "./App";
import { Breadcrumb } from "flowbite-react";
import { useAuth0 } from "@auth0/auth0-react";

export default function AppLayout() {
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
      setDocumentTitle();
    };
    getApp();
  }, []);

  function setDocumentTitle() {
    if (!app) {
      return;
    }
    let title;
    switch (path3) {
      case "overview":
        title = app.name + " - Overview";
        break;
      case "routes":
        title = app.name + " - Routes";
        break;
      case "logs":
        title = app.name + " - Logs";
        break;
      default:
        title = app.name;
        break;
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
              className={`text-xs ${path3 === undefined ? "text-primary" : ""}`}
            >
              Overview
            </Link>
            <Link
              to="routes"
              className={`text-xs ${path3 === "routes" ? "text-primary" : ""}`}
            >
              Routes
            </Link>
            <div className="text-xs">
              Authentications
              <div className="flex flex-col pl-1">
                <Link
                  to="authentications/schemes"
                  className={`text-xs ${
                    path3 === "authentications" && path4 === "schemes"
                      ? "text-primary"
                      : ""
                  }`}
                >
                  Schemes
                </Link>
                <Link
                  to="authentications/settings"
                  className={`text-xs ${
                    path3 === "authentications" && path4 === "settings"
                      ? "text-primary"
                      : ""
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
                    path3 === "storages" && path4 === "textstorages"
                      ? "text-primary"
                      : ""
                  }`}
                >
                  Text Storages
                </Link>
                <Link
                  to="storages/variables"
                  className={`text-xs ${
                    path3 === "storages" && path4 === "variables"
                      ? "text-primary"
                      : ""
                  }`}
                >
                  Variables
                </Link>
              </div>
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
