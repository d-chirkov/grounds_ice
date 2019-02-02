import React from 'react';
import { HashRouter, Route, Switch, RouteComponentProps } from "react-router-dom";

import { HeaderMenu } from "./components/HeaderMenu";
import { ProfileView } from "./components/Profile/ProfileView";

let App = () =>
	<div>
		<HashRouter>
			<div className="main">
				<HeaderMenu />
				<Switch>
					<Route path="/profile/id:userId" component={ProfileView} />
				</Switch>
			</div>
		</HashRouter>
	</div>

export default App;
