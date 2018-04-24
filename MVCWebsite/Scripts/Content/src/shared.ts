class Param {
    name: string;
    value: string;
    constructor(name: string, value: string) {
        this.name = name;
        this.value = value;
    }
    toString(): string {
        if (this.name.length > 1 && this.value.length > 1) {
            return this.name + "=" + this.value;
        }
        return "";
    }
}
function getParamsInLocation(location: string, params: Param[]): string { 
    return location.split('?')[0] + "?" + params.join("&");
}
function getAllParams(paramsString: string): Param[] {
    let params: Param[] = new Array();

    if (paramsString == "") {
        return params;
    }
    let keyValueString = paramsString.split('&');
    //let statement did not work
    for (var i = 0; i < keyValueString.length; i++) {
        let split = keyValueString[i].split('=');

        params.push(new Param(split[0], split[1]));

    }
    return params;
}