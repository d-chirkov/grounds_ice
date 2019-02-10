import fetch from "isomorphic-fetch";
import {Location} from "../Model";
import jsonp from "jsonp"

let vkAccessToken = "27022ac127022ac127022ac149276abbb32270227022ac17b5fdd8ee915467d0beea80e";
let vkApiVersion = "5.92";
let vkGetCitiesMethodName = "database.getCities"
let vkGetCitiesCount = 5;
let vkGetCitiesNeedAll = 1;
let vkGetRegionsMethodName = "database.getRegions"
let vkGetRegionsCount = 2;
let vkCountryId = 1;

export function getLocations(query: string, callback: (locations: Location[]) => void) {
	let citiesRequest = `https://api.vk.com/method/${vkGetCitiesMethodName}?` +
		`country_id=${vkCountryId}&` +
		`q=${query}&` +
		`count=${vkGetCitiesCount}&` +
		`need_all=${vkGetCitiesNeedAll}&` +
		`access_token=${vkAccessToken}&` +
		`v=${vkApiVersion}`;
		
	let regionsRequest = `https://api.vk.com/method/${vkGetRegionsMethodName}?` +
		`country_id=${vkCountryId}&` +
		`q=${query}&` +
		`count=${vkGetRegionsCount}&` +
		`access_token=${vkAccessToken}&` +
		`v=${vkApiVersion}`;
		
	
	jsonp(citiesRequest, (err: any, vkres:any) => {
		let result: Array<Location> = [];
		if (!vkres.hasOwnProperty("response")) {
			callback(result);
		}
		let response = vkres["response"];
		if (!response.hasOwnProperty("items")) {
			callback(result);
		}
		let items = response["items"];
		items.forEach((e:any) => {
			let location: Location = {City: null, Region: null};
			if (e.hasOwnProperty("title")) {
				location.City = e["title"];
				if (e.hasOwnProperty("region")) {
					location.Region = e["region"];
				}
				result.push(location);
			}
		});
		jsonp(regionsRequest, (err: any, vkres:any) => {
			if (!vkres.hasOwnProperty("response")) {
				callback(result);
			}
			let response = vkres["response"];
			if (!response.hasOwnProperty("items")) {
				callback(result);
			}
			let items = response["items"];
			items.forEach((e:any) => {
				let location: Location = {City: null, Region: null};
				if (e.hasOwnProperty("title")) {
					location.Region = e["title"];
					console.log(e["title"]);
					result.push(location);
				}
			});
			callback(result);
		});
	});
	
}