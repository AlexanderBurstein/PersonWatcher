import React, {Component} from 'react';
import { AsyncTypeahead } from 'react-bootstrap-typeahead';

import 'react-bootstrap-typeahead/css/Typeahead.css';
const SEARCH_URI = process.env.REACT_APP_API+'Place';

export interface PlaceOption {
  placeId: number;
  placeName: string;
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

  getPlaceOptions(items:any[]):PlaceOption[] {
    return items.map((i: any) => {
      return {placeId:i.placeId, placeName:i.placeName};
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
        onChange={(sel) => {let selected = this.getPlaceOptions(sel);this.setState({selected});
          this.props.onChangeValue({selected})}}
        options={this.state.options}
        placeholder="Choose a place..."
      />
    );
  }
}

