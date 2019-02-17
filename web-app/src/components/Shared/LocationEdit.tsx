import React from "react";
import * as Model from "../../api/Location/Model"
import { getLocations } from "../../api/Location/interface/getLocation"

import { AutoComplete } from 'primereact/autocomplete';

interface ILocationEditProps {
	intialCity: string
	intialRegion: string
	size?: number
	placeholder? :string
	className?: string
	disabled?: boolean
	onChange?:() => void
	onChangeLocation: (city: string | null, region: string | null) => void
}

interface ILocationEditState {
	locationValue: string
	suggestions?: Model.Location[]
}

class LocationEdit extends React.Component<ILocationEditProps, ILocationEditState> {
	constructor(props: ILocationEditProps) {
		super(props);
		let {intialCity, intialRegion} = props;
		let location: Model.Location = {
			City: intialCity != "" ? intialCity : null,
			Region: intialRegion != "" ? intialRegion : null,
		}
		this.state = {
			locationValue: this.getLocationStringFrom(location)
		}
	}
	
	render() {
		return (
			<AutoComplete 
				disabled={this.props.disabled}
				itemTemplate={this.locationTemplate.bind(this)} 
				value={this.state.locationValue} 
				size={this.props.size}
				inputClassName={this.props.className}
				placeholder={this.props.placeholder}
				onChange={e => {
					if (this.props.onChange) {
						this.props.onChange();
					}
					let value = "";
					if (typeof e.value == "string") {
						value = e.value;
					}
					else {
						let location = e.value as Model.Location;
						if (location.City != null) {
							value = e.value.City;
						}
						if (location.Region != null) {
							value = (value == "") ? e.value.Region : value + ", " + e.value.Region;
						}
					}
					if (value == "") {
						this.props.onChangeLocation(null, null);
					}
					this.setState({locationValue: value});
				}}
				onSelect={e => {this.props.onChangeLocation(e.value.City, e.value.Region)}}
            	suggestions={this.state.suggestions} 
				completeMethod={this.suggestLocations.bind(this)} />
		);
	}
	
	private locationTemplate(location: Model.Location) {
		return (<div>{this.getLocationStringFrom(location)}</div>);
	}
	
	private getLocationStringFrom(location: Model.Location): string {
		let result = "";
		if (location.City != null) {
			result = location.City;
		}
		if (location.Region != null) {
			result = (result == "") ? location.Region : result + ", " + location.Region;
		}
		return result;
	}
	
	private suggestLocations() {
		getLocations(this.state.locationValue, (v) => this.setState({suggestions: v}));
	}
}

export { LocationEdit };