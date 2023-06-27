import React, {Component} from 'react';
import { AsyncTypeahead } from 'react-bootstrap-typeahead';

import 'react-bootstrap-typeahead/css/Typeahead.css';
const SEARCH_URI = process.env.REACT_APP_API+'Place';

export interface PlaceOption {
  placeId: number;
  placeName: string;
  setValue: (value: PlaceOption) => void;
};

type propTypes = {
  [key: string]: any
};
type defaultProps = {
    isLoading: boolean,
    options: PlaceOption[],
    selected: PlaceOption[],
    value: string
};

export default class PlaceTypeahead extends React.Component<propTypes, defaultProps> {
  state = {
    isLoading: false,
    selected: [] as PlaceOption[],
    options: [] as PlaceOption[],
    value: ''
  };

  _handleSearch = (query : string) => {
    this.setState({ isLoading: true });
    this.makeAndHandleRequest(query).then(({ opts }) => {
      this.setState(
        {
          isLoading: false,
          options: opts
        }
      );
    });
  };

  getOptions(items:Array<PlaceOption>) {
    return items.map((i: PlaceOption) => {
      return new Option(i.placeName, i.placeId.toString());
    });
  };

  getPlaceOptions(items:any[]) {
    return items.map((i: any) => {
      var result = new Array<PlaceOption>();
      result.push({placeId:Number(i.value), placeName:i.text, setValue:(value:PlaceOption) => {}});
      this.setState({options: result});
    });
  };
   makeAndHandleRequest(query: string, page = 1) {
    return fetch(`${SEARCH_URI}?searchStr=${query}`)
      .then(resp => resp.json())
      .then(data => {
        const total_count=data.length;
        const opts = data.map((i: { placeId: number; placename: string; latitude: string; longitude: string; }) => ({
          placeId: i.placeId,
          placeName: i.placename + "(" + i.latitude.substring(0, 6) + ", " + i.longitude.substring(0, 7) + ")"
        }));
        return { opts, total_count };
      });
  }

  render() {
    return (
      <AsyncTypeahead
        {...this.state}
        id="placeId"
        labelKey="placeName"
        clearButton
        onSearch={this._handleSearch}
        onChange={(selected) => {this.getPlaceOptions(selected);
          this.props.onChangeValue({selected})}}
        options={this.getOptions(this.state.options)}
        placeholder="Choose a place..."
      />
    );
  }
}

