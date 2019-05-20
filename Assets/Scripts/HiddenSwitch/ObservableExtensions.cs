using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace HiddenSwitch
{
    public static class ObservableExtensions
    {
        /// <summary>
        /// Iterates through the collection on subscription, and then pumps add and replace events.
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IObservable<T> Observe<T>(this IReadOnlyReactiveCollection<T> collection)
        {
            return Observable.Merge(
                collection.ToObservable(),
                collection.ObserveAdd().Select(e => e.Value),
                collection.ObserveReplace().Select(e => e.NewValue));
        }

        /// <summary>
        /// Iterates through the collection on subscription, and pumps add and replace events with the index and the new
        /// document always in the new value field.
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IObservable<CollectionReplaceEvent<T>> ObserveChanges<T>(
            this IReadOnlyReactiveCollection<T> collection)
        {
            return Observable.Merge(
                Observable.Range(0, collection.Count)
                    .Select(i => new CollectionReplaceEvent<T>(i, default(T), collection[i])),
                collection.ObserveAdd().Select(e => new CollectionReplaceEvent<T>(e.Index, default(T), e.Value)),
                collection.ObserveReplace()
            );
        }

        /// <summary>
        /// Returns the item after an async operation has completed from the addressables system. Throws exceptions.
        /// Also completes itself correctly.
        /// </summary>
        /// <param name="op"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IObservable<T> OnCompletedAsObservable<T>(this AsyncOperationHandle<T> op)
        {
            return Observable.FromEvent<Action<AsyncOperationHandle<T>>, T>(
                    h => handle => h(handle.Result),
                    a => op.Completed += a,
                    a => op.Completed -= a)
                .Materialize()
                .SelectMany(notif =>
                {
                    if (notif.HasValue && notif.Value != null)
                    {
                        return new[] {notif, Notification.CreateOnCompleted<T>()};
                    }

                    return new[] {Notification.CreateOnError<T>(op.OperationException)};
                }).Dematerialize();
        }

        /// <summary>
        /// Generates progress notifications for the given download
        /// </summary>
        /// <param name="op"></param>
        /// <param name="frameUpdateFrequency"></param>
        /// <returns></returns>
        public static IObservable<float> OnPercentProgressAsObservable<T>(this AsyncOperationHandle<T> op,
            int frameUpdateFrequency = 5)
        {
            return Observable.IntervalFrame(frameUpdateFrequency)
                .SelectMany(ignored =>
                {
                    if (op.Status == AsyncOperationStatus.None)
                    {
                        return new[] {Notification.CreateOnNext(op.PercentComplete)};
                    }

                    return new[]
                        {Notification.CreateOnNext(op.PercentComplete), Notification.CreateOnCompleted<float>()};
                }).Dematerialize();
        }
    }
}