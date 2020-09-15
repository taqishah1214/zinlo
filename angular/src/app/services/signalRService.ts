import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { HttpClient } from "@angular/common/http";
import * as signalR from "@aspnet/signalr";
import { interval, Subscription } from "rxjs";
import { AppConsts } from "@shared/AppConsts";
import { FileDto } from "@shared/service-proxies/service-proxies";

@Injectable({ providedIn: "root" })
export class SignalRService {
	public hubConnection: signalR.HubConnection;

	constructor() {
		//  const source = Observable.tim(10000);
		// this.subscription = Observable.interval(10000).subscribe(val => this.invokeMethod());
		
	}


	startHubConnection = () => {
			this.hubConnection = new signalR.HubConnectionBuilder()
				.withUrl(
					AppConsts.remoteServiceBaseUrl +"/signalr-job?enc_auth_token="
				)
				.configureLogging(signalR.LogLevel.Information)
				.build();

			this.hubConnection
				.start()
				.then(_ => this.invokeMethod())
				.catch(err =>
					console.log("Error while starting connection: " + err)
				);
			this.addBasicListener();
		
	};

	addBasicListener(): any {
		this.hubConnection.on("chartOfAccount", data => {
			var dt = data;
            return data;
        });
        
        
	};

	invokeMethod = () => {
		
				this.hubConnection
					.invoke("SendMessage", "Invoked")
					.then(_ => console.log("Connection is Still Live!"))
					.catch(_ =>console.log("Error")
					);
		
		
	};

	disconnectHubConnection() {
		this.hubConnection.stop();
	}

	// addGetAddressJobStatusListener = () => {
	// 	console.log("Listeninggggggggggg");
	// 	var dt = "";
	// 	this.hubConnection.on("AddressJobStatus", data => {
	// 		dt = data;
	// 		console.log(dt);

	// 		// if (dt == "Completed") {
	// 		// 	this.wizard.goNext();
	// 		// }

	// 		return dt;
	// 	});
	// 	return dt;
	// };
}
