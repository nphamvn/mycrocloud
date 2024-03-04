import { useAuth0 } from "@auth0/auth0-react";
import { Avatar, Button, Dropdown, Navbar } from "flowbite-react";
import { Link } from "react-router-dom";
const isDevMode = import.meta.env.DEV;

function Header() {
  const {
    isLoading,
    isAuthenticated,
    user,
    loginWithRedirect,
    logout,
    getAccessTokenSilently,
  } = useAuth0();

  const handleCopyAccessTokenClick = async () => {
    const accessToken = await getAccessTokenSilently();
    navigator.clipboard.writeText(accessToken);
  };

  if (isLoading) {
    return null;
  }

  return (
    <Navbar fluid rounded className="border-b-[1px]">
      <Link to="/" className="">
        <span className="self-center whitespace-nowrap text-xl font-semibold">
          MycroCloud
        </span>
      </Link>
      {isAuthenticated ? (
        <>
          <div className="flex md:order-2">
            <Dropdown
              arrowIcon={false}
              inline
              label={<Avatar alt="User settings" img={user?.picture} rounded />}
            >
              <Dropdown.Header>
                <span className="block">{user?.name}</span>
                <span className="block truncate font-medium">
                  {user?.email}
                </span>
              </Dropdown.Header>
              <Dropdown.Item disabled>Settings</Dropdown.Item>
              <Dropdown.Divider />
              <Dropdown.Item onClick={() => logout()}>Log out</Dropdown.Item>
              {isDevMode && (
                <Dropdown.Item onClick={handleCopyAccessTokenClick}>
                  Copy access token
                </Dropdown.Item>
              )}
            </Dropdown>
            <Navbar.Toggle />
          </div>
          <Navbar.Collapse className="items-start">
            <li>
              <Link
                className="block border-b border-gray-100 py-2 pl-3 pr-4 text-gray-700 hover:bg-gray-50 md:border-0 md:p-0 md:hover:bg-transparent md:hover:text-cyan-700 "
                to="apps"
              >
                Apps
              </Link>
            </li>
          </Navbar.Collapse>
        </>
      ) : (
        <Button size="sm" onClick={() => loginWithRedirect()}>
          Log in
        </Button>
      )}
    </Navbar>
  );
}
export default Header;
