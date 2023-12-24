import { useContext } from "react";
import { AppContext } from "./AppContext";

export default function AppOverview() {
  const app = useContext(AppContext)!;
  const domain = `https://app-${app.id}.app.mycrocloud.com`;
  return (
    <>
      <div className="border">
        <table>
          <tbody>
            <tr>
              <td>Name</td>
              <td>{app.name}</td>
            </tr>
            <tr>
              <td>Description</td>
              <td>{app.description}</td>
            </tr>
            <tr>
              <td>Created at</td>
              <td>{new Date(app.createdAt).toDateString()}</td>
            </tr>
            <tr>
              <td>Domain</td>
              <td>{domain}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </>
  );
}
