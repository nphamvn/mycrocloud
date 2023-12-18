import { BrowserRouter, Route, Routes } from "react-router-dom";
import "./App.css";
import Home from "./components/Home";
import AppList from "./components/apps/AppList";
import AppCreate from "./components/apps/AppCreate";
import Header from "./components/Header";
import { Auth0Provider } from "@auth0/auth0-react";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import AppIndex from "./components/apps/AppIndex";
import AppLogs from "./components/apps/AppLog";
import AppOverview from "./components/apps/AppOverview";
import RouteCreateUpdate from "./components/routes/RouteCreateUpdate";
import RouteIndex from "./components/routes/RouteIndex";
import { useEffect } from "react";
import AppConfig from "./constants/AppConfig";

function App() {
  useEffect(() => {
    fetch(`${AppConfig.BASE_API_URL}/api/ping`)
      .then((res) => res.text())
      .then((msg) => console.log(msg));
  }, []);
  return (
    <Auth0Provider
      domain={import.meta.env.VITE_AUTH0_DOMAIN}
      clientId={import.meta.env.VITE_AUTH0_CLIENTID}
      authorizationParams={{ redirect_uri: window.location.origin }}
    >
      <BrowserRouter>
        <Header />
        <div className="container mx-auto min-h-screen bg-slate-50 p-2">
          <Routes>
            <Route path="/" Component={Home} />
            <Route path="/apps" Component={AppList} />
            <Route path="/apps/new" Component={AppCreate} />
            <Route path="/apps/:appId" Component={AppIndex}>
              <Route index path="overview" Component={AppOverview} />
              <Route path="routes" Component={RouteIndex} />
              <Route path="routes/:routeId" Component={RouteIndex} />
              <Route path="routes/new" Component={RouteIndex} />
              <Route path="logs" Component={AppLogs} />
            </Route>
          </Routes>
        </div>
        <ToastContainer />
      </BrowserRouter>
    </Auth0Provider>
  );
}
export default App;
